

public interface IScrollablePanel<T,S>
    where T : System.Enum
    where S : System.Enum
{
    bool CheckActiveMainType(T mainType_IN);
    bool CheckActiveSubType(S subType_IN);
    void SortByEquipmentType(T mainType_IN, SelectorButton<T> buttonIN);
    void SortBySubType (S subType_IN, SelectorButton<S> buttonIN);
}
