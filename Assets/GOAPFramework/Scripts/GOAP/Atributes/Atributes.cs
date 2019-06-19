using System;




///Use to give a name
[AttributeUsage(AttributeTargets.Class)]
public class NameAttribute : Attribute
{
    public string name;
    public NameAttribute(string name)
    {
        this.name = name;
    }
}
///Use to give a name
[AttributeUsage(AttributeTargets.Class)]
public class DescriptionAttribute : Attribute
{
    public string description;
    public DescriptionAttribute(string description)
    {
        this.description = description;
    }
}
///Use to categorization
[AttributeUsage(AttributeTargets.Class)]
public class CategoryAttribute : Attribute
{
    public string category;
    public CategoryAttribute(string category)
    {
        this.category = category;
    }
}
