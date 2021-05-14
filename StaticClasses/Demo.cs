using System.Collections;
using System.Collections.Generic;

enum enumWeapons { leftHand, rightHand, leftFoot, rightFoot };

public static class Demo
{
    public static int currentIndexOfPve = 0;
    public static int Expirience = 0;
    public static int Score = 0;
    public static int MaxScore = 0;

    public static class Inventory {
        public static int MaxHealth = 100;
        public static int MaxMana = 100;
        public static int Force = 5;
        public static int Gold = 0;
        public static Dictionary<int, string> slotsOfWeaponInInventory =
                                    new Dictionary<int, string>() {
                                        { (int)enumWeapons.leftHand, "" },
                                        { (int)enumWeapons.rightHand, "" },
                                        { (int)enumWeapons.leftFoot, "" },
                                        { (int)enumWeapons.rightFoot, "" },
                                    };

        public static Dictionary<int, string> slotsOfActions = new Dictionary<int, string>() {
            { 0, "" },
            { 1, "" },
            { 2, "" },
            { 3, "" },
            { 4, "" }
        };
    }
}
