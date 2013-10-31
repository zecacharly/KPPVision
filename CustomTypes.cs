using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using IOModule;

namespace VisionModule {


    public class Customconverter<T> : ExpandableObjectConverter {

        public override object ConvertTo(ITypeDescriptorContext context,
                                 System.Globalization.CultureInfo culture,
                                 object value, Type destType) {
            if (destType == typeof(string) && value is BlobInfo) {
                T blob = (T)value;
                return "";
            }
            return base.ConvertTo(context, culture, value, destType);
        }


    }


    public class CustomCollection<T> : CollectionBase, ICustomTypeDescriptor {


        private String _name = "List";

        public void SetName(String Name) {
            _name = Name;
        }

        public Object GetList() {
            return List;
        }

        

        #region  Collection methods implementation

        public void Add(T source) {
            this.List.Add(source);
        }
        public void Remove(T source) {
            this.List.Remove(source);
        }
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public T this[int index] {
            get {
                try {
                    if (index < this.List.Count) {
                        //index--;

                        return (T)this.List[index];
                    } else {
                        return default(T);
                    }
                } catch (Exception exp) {

                    return default(T);

                }
            }
        }

        #endregion

        public List<T> ToList() {
            List<T> list = new List<T>();
            foreach (T item in this.List) {
                list.Add(item);
            }

            return list;
        }

        #region Implementation of ICustomTypeDescriptor:

        public String GetClassName() {
            return TypeDescriptor.GetClassName(this, true);
        }

        public AttributeCollection GetAttributes() {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public String GetComponentName() {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter() {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent() {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty() {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType) {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes) {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents() {
            return TypeDescriptor.GetEvents(this, true);
        }



        public object GetPropertyOwner(PropertyDescriptor pd) {
            return this;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties() {
            // Create a new collection object PropertyDescriptorCollection
            PropertyDescriptorCollection pds = new PropertyDescriptorCollection(null);

            // Iterate the list of employees
            for (int i = 0; i < this.List.Count; i++) {
                // For each employee create a property descriptor 
                // and add it to the 
                // PropertyDescriptorCollection instance
                CustomCollectionPropertyDescriptor<T> pd = new
                              CustomCollectionPropertyDescriptor<T>(this, i);
                pds.Add(pd);
            }
            return pds;
        }

        #endregion

        public override string ToString() {

            return this._name;
        }
    }

    public class CustomCollectionPropertyDescriptor<T> : PropertyDescriptor {
        private CustomCollection<T> collection = null;
        private int index = -1;

        public CustomCollectionPropertyDescriptor(CustomCollection<T> coll,
                           int idx)
            : base("#" + idx.ToString(), null) {
            this.collection = coll;
            this.index = idx;
        }

        public override AttributeCollection Attributes {
            get {
                return new AttributeCollection(null);
            }
        }


        public override bool CanResetValue(object component) {
            return true;
        }

        public override Type ComponentType {
            get {
                return this.collection.GetType();
            }
        }

        public override string DisplayName {
            get {
                try {
                    T source = this.collection[index];
                    if (source != null) {


                        String Name = source.ToString();//(String)source.GetType().GetProperty("Name").GetValue(source, null);
                        if (Name != "") {
                            return Name;
                        }
                        return source.ToString();
                    } else {
                        return "";
                    }
                } catch (Exception exp) {
                    return null;
                }
            }
        }

        public override string Description {
            get {

                T source = this.collection[index];
                if (source == null) {
                    return "";
                }
                try {
                    String Name = (String)source.GetType().GetProperty("Name").GetValue(source, null);
                    if (Name != "") {
                        return Name;
                    }
                } catch (Exception exp) {


                }
                return source.ToString();
            }
        }

        public override object GetValue(object component) {
            return this.collection[index];
        }

        public override bool IsReadOnly {
            get { return false; }
        }

        public override string Name {
            get { return "#" + index.ToString(); }
        }

        public override Type PropertyType {
            get {
                if (this.collection[index] == null) {
                    return typeof(Nullable);
                }
                return this.collection[index].GetType();
            }
        }

        public override void ResetValue(object component) { }

        public override bool ShouldSerializeValue(object component) {
            return true;
        }



        public override void SetValue(object component, object value) {
            //this.collection[index] = value;
        }
    }

   
}
