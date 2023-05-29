using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;
using System.Reflection;
using System.IO;

namespace RestoreMonarchy.CustomCommands.Storage
{
    public class DatFast
    {

        /// <summary>
        /// Array of strings, represents each line in the file.
        /// </summary>
        public string[] Lines;

        /// <summary>
        /// Path to file that is being modified/read.
        /// </summary>
        public string Path;

        /// <summary>
        /// What is queued for write.
        /// </summary>
        public List<string> Writes;

        /// <summary>
        /// Current line DatFast is reading.
        /// </summary>
        public int Line;

        /// <summary>
        /// Could DatFast find the file? Is it safe to read?
        /// </summary>
        public bool Safe => Lines != null;

        /// <summary>
        /// Could DatFast find the file? Is it safe to read with current order?
        /// </summary>
        public bool SafeOrder => Lines != null && Lines.Length >= (Line + 1);

        /// <summary>
        /// Based partially on the concept of Unturned's .dat files. This system is meant to be lightweight.
        /// </summary>
        /// <param name="path">Specifies which file should be loaded.</param>
        public DatFast(string path)
        {
            Path = path;
            if (File.Exists(path))
            {
                Lines = File.ReadAllLines(path);
            }
            else
            {
                Lines = null;
            }
            Writes = new List<string>();
            Line = 0;
        }

        public T Read<T>()
        {
            if (!SafeOrder) return default;

            var method = typeof(T).GetMethod("Parse", BindingFlags.Static);

            if (method != null)
            {
                try
                {
                    T x = (T)method.Invoke(null, new[] { Lines[Line] });
                    Line++;
                    return x;
                }
                catch
                {
                    return default;
                }
            }
            else
            {
                return default;
            }
        }

        public T Read<T>(string key)
        {
            if (!SafeOrder) return default;

            string[] array = Lines[Line].Split(new[] { ' ' }, 1);
            if (array.Length < 2 || array.Length > 2) return default;

            var method = typeof(T).GetMethod("Parse", BindingFlags.Static);

            if (method != null)
            {
                try
                {
                    T x = (T)method.Invoke(null, new[] { array[1] });
                    Line++;
                    return x;
                }
                catch
                {
                    return default;
                }
            }
            else
            {
                return default;
            }
        }

        public T ReadNonOrder<T>(string key)
        {
            if (!Safe) return default;

            foreach (string line in Lines)
            {
                string[] array = Lines[Line].Split(new[] { ' ' }, 1);
                if (array.Length >= 2 && array.Length < 3)
                {
                    if (array[0] == key)
                    {
                        var method = typeof(T).GetMethod("Parse", BindingFlags.Static);

                        if (method != null)
                        {
                            try
                            {
                                T x = (T)method.Invoke(null, new[] { array[1] });
                                return x;
                            }
                            catch
                            {
                                return default;
                            }
                        }
                        else
                        {
                            return default;
                        }
                    }
                }
            }
            return default;
        }
        
        public void ResetOrder()
        {
            Line = 0;
        }

        public void ResetWrites()
        {
            Writes.Clear();
        }

        public void Write<T>(T obj) where T : IConvertible
        {
            Writes.Add(obj.ToString());
        }

        public void Write<T>(string key, T obj) where T : IConvertible
        {
            Writes.Add($"{key} {obj}");
        }

        public void Save()
        {
            File.WriteAllLines(Path, Writes.ToArray());
        }
    }
}
