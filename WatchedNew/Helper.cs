using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Core {
    public class Helper {

        /// <summary>
        /// Verify that the property name matches a real, public, instance property on the object.
        /// </summary>
        /// <param name="Current"></param>
        /// <param name="PropertyName"></param>
        /// <param name="ThrowInvalidPropertyName"></param>
        public static void VerifyPropertyName(object Current, string PropertyName, bool ThrowInvalidPropertyName = true) {

            if (TypeDescriptor.GetProperties(Current)[PropertyName] == null) {
                string msg = string.Format("Invalid property name \"{0}\" of type {1}", PropertyName, Current.GetType().Name);
                if (ThrowInvalidPropertyName) {
                    throw new Exception(msg);
                }
                else {
                    System.Diagnostics.Debug.Fail(msg);
                }

            }

        }



        public static string DataFilePathDefault() {
            return Path.Combine(FilePathDefault(), "data.gz");
        }

        public static string FilePathDefault() {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

    }
}
