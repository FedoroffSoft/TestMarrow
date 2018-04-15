# TestMarrow
A library to simplify the test data composing in the source code of tests. When you need fill a comples structures for test then you have to write something like this using c#:

    var expected = new List<Struct1> {
        new Struct1 { Name1 = "Marrow", Value1 = 13.07,
            EnumStrcutProp1 = new List<Struct2> {
                new Struct2 {Name2 = "SubMarrow1", Value2 = 4 },
                new Struct2 {Name2 = "SubMarrow2", Value2 = 5 },
            }
        },
        new Struct1 { Value1 = -12 },
        new Struct1 { Name1 = String.Empty, Value1 = -0.43 }
    };


TestMarrow allows you to create the same object using this syntax:

    String str = @">| Name1             | Value1        | 
                    | Marrow            | 13.07         |
                   >| EnumStrcutProp1   | Name2         | Value2 | 
                    |                   | SubMarrow1    | 4      |
                    |                   | SubMarrow2    | 5      |
                    | NULL              | -12           |
                    | ""                | -.43          |
            ";
    
    var parser = new MarrowParser();
    var actual = parser.Parse<List<Struct1>>(str);
