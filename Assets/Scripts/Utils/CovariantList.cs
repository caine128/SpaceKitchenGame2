using System.Collections;
using System.Collections.Generic;


//public class CovariantList<T_Class, T_Interface> : IEnumerable<T_Interface>
//    where T_Class : T_Interface
//{
//    private readonly List<T_Class> list;

//    public CovariantList(List<T_Class> incoming_List)
//    {
//        this.list = incoming_List;
//    }

//    public IEnumerator<T_Interface> GetEnumerator()
//    {
//        foreach (T_Class listItem in list)
//        {
//            yield return listItem;
//        }
//    }

//    IEnumerator IEnumerable.GetEnumerator()
//    {
//        return this.GetEnumerator();
//    }
//}
