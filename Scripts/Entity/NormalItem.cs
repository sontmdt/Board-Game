using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalItem : Item
{
    public eNormalType ItemType;
    public void SetType(eNormalType type)
    {
        ItemType = type;
    }
    protected override string GetPrefabName()
    {
        string prefabName = string.Empty;
        switch (ItemType)
        {
            case eNormalType.TYPE_ONE:
                prefabName = Constants.PREFAB_NORMAL_TYPE_ONE;
                break;
            case eNormalType.TYPE_TWO:
                prefabName = Constants.PREFAB_NORMAL_TYPE_TWO;
                break;
            case eNormalType.TYPE_THREE:
                prefabName = Constants.PREFAB_NORMAL_TYPE_THREE;
                break;
            case eNormalType.TYPE_FOUR:
                prefabName = Constants.PREFAB_NORMAL_TYPE_FOUR;
                break;
            case eNormalType.TYPE_FIVE:
                prefabName = Constants.PREFAB_NORMAL_TYPE_FIVE;
                break;
        }
        return prefabName;
    }
    internal override bool IsSameType(Item other)
    {
        NormalItem item = other as NormalItem;
        return item != null && item.ItemType == this.ItemType;
    }
}
