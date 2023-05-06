using MelonLoader;
using HarmonyLib;
using Il2Cpp;
using gospel_item;
using Il2Cppfacility_H;
using Il2Cppnewdata_H;

[assembly: MelonInfo(typeof(GospelItem), "Gospel item", "1.0.0", "Matthiew Purple")]
[assembly: MelonGame("アトラス", "smt3hd")]

namespace gospel_item;
public class GospelItem : MelonMod
{
    // After creating the shop
    [HarmonyPatch(typeof(fclShopCalc), nameof(fclShopCalc.shpCreateItemList))]
    private class Patch
    {
        public static void Postfix(ref fclDataShop_t pData)
        {
            // Adds the gospel to the shop
            pData.BuyItemList[pData.BuyItemCnt++] = 60;
        }
    }

    // After getting the name of a skill
    [HarmonyPatch(typeof(datItemName), nameof(datItemName.Get))]
    private class Patch2
    {
        public static void Postfix(ref int id, ref string __result)
        {
            // If searching for the gospel, returns its name
            if (id == 60) __result = "Gospel";
        }
    }

    // After getting the name of a skill
    [HarmonyPatch(typeof(datItemHelp_msg), nameof(datItemHelp_msg.Get))]
    private class Patch3
    {
        public static void Postfix(ref int id, ref string __result)
        {
            // If searching for the gospel, returns its description
            if (id == 60) __result = "Demi-fiend earns enough EXP \nto level up but loses one level.";
        }
    }

    // After apply a skill effect outside of battle
    [HarmonyPatch(typeof(datCalc), nameof(datCalc.datExecSkill))]
    private class Patch4
    {
        public static void Postfix(ref int nskill)
        {
            // If using a gospel
            if (nskill == 95)
            {
                datUnitWork_t unit = dds3GlobalWork.DDS3_GBWK.unitwork[0];
                if (unit.level > 1)
                {
                    unit.level--;
                    unit.exp = rstCalcCore.GetNextExpDisp(unit, 0) - 1;

                    bool hasLostStat = false;

                    while (!hasLostStat)
                    {
                        List<short> statList = new List<short> { 0, 2, 3, 4, 5 };

                        Random rnd = new Random();
                        short stat = statList[rnd.Next(statList.Count)];

                        if (unit.param[stat] > 1)
                        {
                            unit.param[stat]--;
                            hasLostStat = true;
                        }
                        else
                        {
                            statList.Remove(stat);
                        }
                    }

                    dds3GlobalWork.DDS3_GBWK.unitwork[0] = unit;
                }
            }
        }
    }

    // When launching the game
    public override void OnInitializeMelon()
    {
        // Creates the item
        datItem.tbl[60].flag = 4; // Normal item
        datItem.tbl[60].price = 1000; // 1000 macca each
        datItem.tbl[60].skillid = 95; // Triggers the skill n°95
        datItem.tbl[60].use = 1; // Can only be used out of battle

        // Creates the skill n°95
        datSkill.tbl[95].capacity = 4;
        datSkill.tbl[95].skillattr = 15; // Utility skill

        datNormalSkill.tbl[95].koukatype = 1; // Not Physical
        datNormalSkill.tbl[95].program = 14; // Phase shift
        datNormalSkill.tbl[95].targetcntmax = 1;
        datNormalSkill.tbl[95].targetcntmin = 1;
        datNormalSkill.tbl[95].targettype = 3; // Field
    }
}
