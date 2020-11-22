using System;
using System.Collections.Generic;
using System.Xml;

namespace Explorer
{
    class DOM
    {
        public List<Students> search(Students s, string path)
        {
            List<Students> result = new List<Students>();
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode node = doc.DocumentElement;
            foreach(XmlNode nod in node.ChildNodes)
            {
                string Name = "";
                string Faculty = "";
                string Group = "";
                string Subject = "";
                string Mark = "";
                foreach(XmlAttribute attribute in nod.Attributes)
                {
                    if(attribute.Name.Equals("Name") && (attribute.Value.Equals(s.Name) || s.Name.Equals(String.Empty)))
                    {
                        Name = attribute.Value;
                    }
                    if (attribute.Name.Equals("Faculty") && (attribute.Value.Equals(s.Faculty) || s.Faculty.Equals(String.Empty)))
                    {
                        Faculty = attribute.Value;
                    }
                    if (attribute.Name.Equals("Group") && (attribute.Value.Equals(s.Group) || s.Group.Equals(String.Empty)))
                    {
                        Group = attribute.Value;
                    }
                    if (attribute.Name.Equals("Subject") && (attribute.Value.Equals(s.Subject) || s.Subject.Equals(String.Empty)))
                    {
                        Subject = attribute.Value;
                    }
                    if (attribute.Name.Equals("Mark") && (attribute.Value.Equals(s.Mark) || s.Mark.Equals(String.Empty)))
                    {
                        Mark = attribute.Value;
                    }
                }

                if(Name != "" && Faculty !="" && Group != "" && Subject != "" && Mark != "")
                {
                    Students student = new Students();
                    student.Name = Name;
                    student.Faculty = Faculty;
                    student.Group = Group;
                    student.Subject = Subject;
                    student.Mark = Mark;
                    result.Add(student);
                }

            }
            return result;
        }
        
    }
}
