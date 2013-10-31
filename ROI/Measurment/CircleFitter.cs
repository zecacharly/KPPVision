using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace VisionModule {


    public class CircleFitter2 {

        public int CircleFit(int N, Point[] P,ref double pa, ref double pb, ref double pr) {
            /* user-selected parameters */
            const int maxIterations = 256;
            const double tolerance = 1e-06;

            double a, b, r=0;

            /* compute the average of the data points */
            int i, j;
            double xAvr = 0.0;
            double yAvr = 0.0;

            for (i = 0; i < N; i++) {
                xAvr += P[i].X;
                yAvr += P[i].Y;
            }
            xAvr /= N;
            yAvr /= N;

            /* initial guess */
            a = xAvr;
            b = yAvr;

            for (j = 0; j < maxIterations; j++) {
                /* update the iterates */
                double a0 = a;
                double b0 = b;

                /* compute average L, dL/da, dL/db */
                double LAvr = 0.0;
                double LaAvr = 0.0;
                double LbAvr = 0.0;

                for (i = 0; i < N; i++) {
                    double dx = P[i].X - a;
                    double dy = P[i].Y - b;
                    double L = Math.Sqrt(dx * dx + dy * dy);
                    if (Math.Abs(L) > tolerance) {
                        LAvr += L;
                        LaAvr -= dx / L;
                        LbAvr -= dy / L;
                    }
                }
                LAvr /= N;
                LaAvr /= N;
                LbAvr /= N;

                a = xAvr + LAvr * LaAvr;
                b = yAvr + LAvr * LbAvr;
                r = LAvr;

                if (Math.Abs(a - a0) <= tolerance && Math.Abs(b - b0) <= tolerance)
                    break;
            }

            pa = a;
            pb = b;
            pr = r;

            return (j < maxIterations ? j : -1);
        }


        public CircleFitter2() {
        }
    }


    public class CircleFitter3 {



        public CircleFitter3() {
        }
    }

    public class CircleFitter {


        public CircleFitter() {
            center = new Point(0, 0);
            rHat = 1.0;
            points = null;
        }


        /** Initialize an approximate circle based on all triplets.
         * @param points circular ring sample points
         * @exception LocalException if all points are aligned
         */
        public void initialize(Point[] points) {

            // store the points array
            this.points = points;

            // analyze all possible points triplets
            center.X = 0;
            center.Y = 0;
            int n = 0;
            for (int i = 0; i < (points.Length - 2); ++i) {
                Point p1 = (Point)points[i];
                for (int j = i + 1; j < (points.Length - 1); ++j) {
                    Point p2 = (Point)points[j];
                    for (int k = j + 1; k < points.Length; ++k) {
                        Point p3 = (Point)points[k];

                        // compute the triangle circumcenter
                        Point cc = circumcenter(p1, p2, p3);
                        if (cc != null) {
                            // the points are not aligned, we have a circumcenter
                            ++n;
                            center.X += cc.X;
                            center.Y += cc.Y;
                        }
                    }
                }
            }

            if (n == 0) {
                //throw new LocalException("all points are aligned");
            }

            // initialize using the circumcenters average
            center.X /= n;
            center.Y /= n;
            updateRadius();

        }

        /** Update the circle radius.
         */
        private void updateRadius() {
            rHat = 0;
            for (int i = 0; i < points.Length; ++i) {
                double dx = points[i].X - center.X;
                double dy = points[i].Y - center.Y;
                rHat += Math.Sqrt(dx * dx + dy * dy);
            }
            rHat /= points.Length;
        }

        /** Compute the circumcenter of three points.
         * @param pI first point
         * @param pJ second point
         * @param pK third point
         * @return circumcenter of pI, pJ and pK or null if the points are aligned
         */
        private Point circumcenter(Point pI, Point pJ, Point pK) {

            // some temporary variables
            Point dIJ = new Point(pJ.X - pI.X, pJ.Y - pI.Y);
            Point dJK = new Point(pK.X - pJ.X, pK.Y - pJ.Y);
            Point dKI = new Point(pI.X - pK.X, pI.Y - pK.Y);
            double sqI = pI.X * pI.X + pI.Y * pI.Y;
            double sqJ = pJ.X * pJ.X + pJ.Y * pJ.Y;
            double sqK = pK.X * pK.X + pK.Y * pK.Y;

            // determinant of the linear system: 0 for aligned points
            double det = dJK.X * dIJ.Y - dIJ.X * dJK.Y;
            if (Math.Abs(det) < 1.0e-10) {
                // points are almost aligned, we cannot compute the circumcenter
                return new Point();
            }

            // beware, there is a minus sign on Y coordinate!
            return new Point((int)((sqI * dJK.Y + sqJ * dKI.Y + sqK * dIJ.Y) / (2 * det)),
                  -(int)((sqI * dJK.X + sqJ * dKI.X + sqK * dIJ.X) / (2 * det)));

        }

        /** Minimize the distance residuals between the points and the circle.
         * <p>We use a non-linear conjugate gradient method with the Polak and
         * Ribiere coefficient for the computation of the search direction. The
         * inner minimization along the search direction is performed using a
         * few Newton steps. It is worthless to spend too much time on this inner
         * minimization, so the convergence threshold can be rather large.</p>
         * @param maxIter maximal iterations number on the inner loop (cumulated
         * across outer loop iterations)
         * @param innerThreshold inner loop threshold, as a relative difference on
         * the cost function value between the two last iterations
         * @param outerThreshold outer loop threshold, as a relative difference on
         * the cost function value between the two last iterations
         * @return number of inner loop iterations performed (cumulated
         * across outer loop iterations)
         * @exception LocalException if we come accross a singularity or if
         * we exceed the maximal number of iterations
         */
        public int minimize(int iterMax, double innerThreshold, double outerThreshold) {

            computeCost();
            if ((J < 1.0e-10) || (Math.Sqrt(dJdx * dJdx + dJdy * dJdy) < 1.0e-10)) {
                // we consider we are already at a local minimum
                return 0;
            }

            double previousJ = J;
            double previousU = 0.0, previousV = 0.0;
            double previousDJdx = 0.0, previousDJdy = 0.0;
            for (int iterations = 0; iterations < iterMax; ) {

                // search direction
                double u = -dJdx;
                double v = -dJdy;
                if (iterations != 0) {
                    // Polak-Ribiere coefficient
                    double beta =
                      (dJdx * (dJdx - previousDJdx) + dJdy * (dJdy - previousDJdy))
                    / (previousDJdx * previousDJdx + previousDJdy * previousDJdy);
                    u += beta * previousU;
                    v += beta * previousV;
                }
                previousDJdx = dJdx;
                previousDJdy = dJdy;
                previousU = u;
                previousV = v;

                // rough minimization along the search direction
                double innerJ;
                do {
                    innerJ = J;
                    double lambda = newtonStep(u, v);
                    center.X += (int)(lambda * u);
                    center.Y += (int)(lambda * v);
                    updateRadius();
                    computeCost();
                } while ((++iterations < iterMax)
                         && ((Math.Abs(J - innerJ) / J) > innerThreshold));

                // global convergence test
                if ((Math.Abs(J - previousJ) / J) < outerThreshold) {
                    return iterations;
                }
                previousJ = J;

            }

            return -1;
            //throw new LocalException("unable to converge after "
            //                         + iterMax + " iterations");

        }

        /** Compute the cost function and its gradient.
         * <p>The results are stored as instance attributes.</p>
         */
        private void computeCost() {
            J = 0;
            dJdx = 0;
            dJdy = 0;
            for (int i = 0; i < points.Length; ++i) {
                double dx = points[i].X - center.X;
                double dy = points[i].Y - center.Y;
                double di = Math.Sqrt(dx * dx + dy * dy);
                if (di < 1.0e-10) {
                    //throw new LocalException("cost singularity:"
                    //                         + " point at the circle center");
                }
                double dr = di - rHat;
                double ratio = dr / di;
                J += dr * (di + rHat);
                dJdx += dx * ratio;
                dJdy += dy * ratio;
            }
            dJdx *= 2.0;
            dJdy *= 2.0;
        }

        /** Compute the length of the Newton step in the search direction.
         * @param u abscissa of the search direction
         * @param v ordinate of the search direction
         * @return value of the step along the search direction
         */
        private double newtonStep(double u, double v) {

            // compute the first and second derivatives of the cost
            // along the specified search direction
            double sum1 = 0, sum2 = 0, sumFac = 0, sumFac2R = 0;
            for (int i = 0; i < points.Length; ++i) {
                double dx = center.X - points[i].X;
                double dy = center.Y - points[i].Y;
                double di = Math.Sqrt(dx * dx + dy * dy);
                double coeff1 = (dx * u + dy * v) / di;
                double coeff2 = di - rHat;
                sum1 += coeff1 * coeff2;
                sum2 += coeff2 / di;
                sumFac += coeff1;
                sumFac2R += coeff1 * coeff1 / di;
            }

            // step length attempting to nullify the first derivative
            return -sum1 / ((u * u + v * v) * sum2
                            - sumFac * sumFac / points.Length
                            + rHat * sumFac2R);

        }

        /** Get the circle center.
         * @return circle center
         */
        public Point getCenter() {
            return center;
        }

        /** Get the circle radius.
         * @return circle radius
         */
        public double getRadius() {
            return rHat;
        }



        /** Current circle center. */
        private Point center;

        /** Current circle radius. */
        private double rHat;

        /** Circular ring sample points. */
        private Point[] points;

        /** Current cost function value. */
        private double J;

        /** Current cost function gradient. */
        private double dJdx;
        private double dJdy;

    }
}
