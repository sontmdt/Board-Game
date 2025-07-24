using System;
using System.Linq;
using UnityEngine;

public class Utils
{
    public static eNormalType RandomNormalType()
    {
        eNormalType[] types = (eNormalType[])Enum.GetValues(typeof(eNormalType));
        int randomIndex = UnityEngine.Random.Range(0, types.Length);
        return types[randomIndex];
    }
    public static eNormalType RandomNormalTypeExcept(eNormalType[] exceptTypes)
    {
        eNormalType[] types = Enum.GetValues(typeof(eNormalType))
            .Cast<eNormalType>()
            .Except(exceptTypes)
            .ToArray();

        int randomIndex = UnityEngine.Random.Range(0, types.Length);
        return types[randomIndex];
    }

}
