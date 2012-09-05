﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Panta.Fetchers;
using Panta.Indexing;

namespace Panta.DataModels
{
    /// <summary>
    /// A school that offers courses to students
    /// </summary>
    [Serializable]
    public class School : IName
    {
        public string Name { get; set; }
        public string Abbr { get; set; }

        /// <summary>
        /// Final storage of the couses once got from the department
        /// </summary>
        protected Dictionary<uint, Course> CoursesCatalog { get; set; }

        public IEnumerable<Course> Courses { get { return CoursesCatalog.Values; } }

        public School(string name, string abbr, IdSigner<Course> signer, IEnumerable<Course> courses)
        {
            this.Name = name;
            this.Abbr = abbr;
            this.CoursesCatalog = new Dictionary<uint, Course>();
            foreach (Course course in courses)
            {
                // Generate course universal id as the key (not the course name any more)
                this.CoursesCatalog.Add(signer.SignId(course), course);
            }
        }

        public bool TryGetCourse(uint id, out Course course)
        {
            return this.CoursesCatalog.TryGetValue(id, out course);
        }

        #region Read/Save
        // Read a school from a serialized binary file
        public static School Read(string path)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            School obj = (School)formatter.Deserialize(stream);
            stream.Close();
            return obj;
        }

        // Save the instance to a serialized binary file
        public void Save()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(this.Abbr + ".bin", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this);
            stream.Close();
        }
        #endregion
    }
}
