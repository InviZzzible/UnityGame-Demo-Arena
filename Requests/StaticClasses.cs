using System;
using System.Collections;
using System.Collections.Generic;


public static class StaticClasses
{
    public static class TransitsData { // Из этого класса будем получать глобальные данные из БД:

        public static string transitNickname = "";
        public static string transitPassword = "";
        public static int transitNumberOfModel2D = 0;

        public static int transitPveCurrentScore = 0;
        public static int transitPveMaxScore = 0;
        public static int transitPveCurrentLevel = 1;

        public static int transitHealth = 100;
        public static int transitMaxHealth = 100;
        public static int transitForce = 10;

        // Позиция персонажа на сцене (отправляем только при онлайн pvp режиме игры):
        public static float HeadPosX = 0f;
        public static float HeadPosY = 0f;
        public static float HeadRot = 0f;

        public static float BodyPosX = 0f;
        public static float BodyPosY = 0f;
        public static float BodyRot = 0f;

        public static float LeftShoulderPosX = 0f;
        public static float LeftShoulderPosY = 0f;
        public static float LeftShoulderRot = 0f;

        public static float RightShoulderPosX = 0f;
        public static float RightShoulderPosY = 0f;
        public static float RightShoulderRot = 0f;

        public static float LeftHandPosX = 0f;
        public static float LeftHandPosY = 0f;
        public static float LeftHandRot = 0f;

        public static float RightHandPosX = 0f;
        public static float RightHandPosY = 0f;
        public static float RightHandRot = 0f;

        public static float LeftLegPosX = 0f;
        public static float LeftLegPosY = 0f;
        public static float LeftLegRot = 0f;

        public static float RightLegPosX = 0f;
        public static float RightLegPosY = 0f;
        public static float RightLegRot = 0f;

        public static float LeftFootPosX = 0f;
        public static float LeftFootPosY = 0f;
        public static float LeftFootRot = 0f;

        public static float RightFootPosX = 0f;
        public static float RightFootPosY = 0f;
        public static float RightFootRot = 0f;

        // -------------------------------------------------------

        public class Store {
            public string transitThingName = "";
            public int transitThingPrice = 0;
        }
        public static List<Store> ThingsOfStore;

        public static DateTime lastVisit;

    }
}
