using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

public class TestAtrribute : MonoBehaviour {

    private void Start()
    {
        Type ag = typeof(StudentA);
        string name1 = ag.Name;
        Debug.Log(name1);

        object[] os = ag.GetCustomAttributes(typeof(StudentAttribute), false);
        StudentAttribute sa = (StudentAttribute)os[0];
        Debug.Log(sa.name);
        Debug.Log(sa.score);

        FieldInfo[] fields = ag.GetFields();

        for (int i = 0, maxi = fields.Length; i < maxi; ++i)
        {
            //可以有多个
            object[] fieldOS = fields[i].GetCustomAttributes(typeof(StudentFieldAttribute), true);
            if (fieldOS.Length > 0)
            {
                StudentFieldAttribute sfa = (StudentFieldAttribute)fieldOS[0];
                if (sfa != null)
                    Debug.Log(sfa.age);
            }
        }
    }
}

[Student("zhangsan",100)]
public class StudentA 
{
    [StudentField(10)]
    public int age1;

    [StudentField(20)]
    public int age2;

    public int age3;
}

public class StudentFieldAttribute : Attribute
{
    public int age { get; private set; }

    public StudentFieldAttribute(int age)
    {
        this.age = age;
    }
}

public class StudentAttribute : Attribute
{
    public string name { get; private set; }

    public int score { get; private set; }

    public StudentAttribute(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}
