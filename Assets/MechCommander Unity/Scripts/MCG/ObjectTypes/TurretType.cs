using UnityEngine;
using UnityEditor;
using MechCommanderUnity.API;
using System;

namespace MechCommanderUnity.MCG.ObjectTypes
{
    [Serializable]
    public class TurretType : ObjectType
    {
        #region Class Variables
        static int MAX_TURRET_WEAPONS = 4;

        public float damageLevel;
        public int blownEffectId;
        public int normalEffectId;
        public int damageEffectId;

        public float baseTonnage;

        public float explDmg;
        public float explRad;

        public bool blocksLineOfFire;

        public float engageRadius;
        public float turretYawRate;
        public int[] weaponMasterId = new int[MAX_TURRET_WEAPONS];
        public int pilotSkill;
        public float punch;

        public int lowTemplate;
        public int highTemplate;

        public int turretTypeName;

        #endregion

        #region Class Structures
        #endregion

        #region Constructors

        public TurretType(FITFile objFitFile) : base(objFitFile)
        {
            objectTypeClass = (int)ObjectTypeClass.TURRET_TYPE;
            objectClass = ObjectClass.TURRET;
            damageLevel = 0f;
            blownEffectId = -1;
            normalEffectId = -1;
            damageEffectId = -1;
            explDmg = explRad = 0f;
            baseTonnage = 0f;
            weaponMasterId[0] = weaponMasterId[1] = weaponMasterId[2] = weaponMasterId[3] = -1;
            pilotSkill = 0;
            punch = 0f;
            turretYawRate = 0f;
            turretTypeName = 0;
            blocksLineOfFire = false;

            lowTemplate = 0;
            highTemplate = 0;


            if (!objFitFile.SeekSection("TurretData"))
                return;

            objFitFile.GetBool("BlocksLineOfFire", out blocksLineOfFire);

            objFitFile.GetFloat("DmgLevel", out damageLevel);

            if (!objFitFile.GetInt("BlownEffectId", out blownEffectId))
                blownEffectId = -1;
            if (!objFitFile.GetInt("NormalEffectId", out normalEffectId))
                normalEffectId = -1;
            if (!objFitFile.GetInt("DamageEffectId", out damageEffectId))
                damageEffectId = -1;

            objFitFile.GetInt("LowTemplate", out lowTemplate);
            objFitFile.GetInt("HighTemplate", out highTemplate);

            if (!objFitFile.GetFloat("ExplosionRadius", out explRad))
                explRad = 0f;// if this fails, explosion radius is not set and no splash damage.

            if (!objFitFile.GetFloat("ExplosionDamage", out explDmg))
                explDmg = 0f; // if this fails, explosion damage is not set and no splash damage.



            if (!objFitFile.GetFloat("Tonnage", out baseTonnage))
                baseTonnage = 20f;

            if (!objFitFile.GetFloat("AttackRadius", out engageRadius))
                return;

            if (!objFitFile.GetFloat("MaxTurretYawRate", out turretYawRate))
                return;
            if (!objFitFile.GetInt("WeaponType", out weaponMasterId[0]))
                return;


            //SPOTLIGHTS!!!!
            if (weaponMasterId[0] != -1)
                punch = 99999f;//TODO: MASTERCOMPONENTLIST MasterComponent::masterList[weaponMasterId[0]].getCV();
            else
                punch = 0.0f;

            if (!objFitFile.GetInt("WeaponType1", out weaponMasterId[1]))
                weaponMasterId[1] = -1;
            else
                punch += 99999f;//TODO: MasterComponent::masterList[weaponMasterId[1]].getCV();

            if (!objFitFile.GetInt("WeaponType2", out weaponMasterId[2]))
                weaponMasterId[2] = -1;
            else
                punch += 99999f;//TODO: MasterComponent::masterList[weaponMasterId[2]].getCV();

            if (!objFitFile.GetInt("WeaponType3", out weaponMasterId[3]))
                weaponMasterId[3] = -1;
            else
                punch += 99999f;//TODO: MasterComponent::masterList[weaponMasterId[3]].getCV();

            if (!objFitFile.GetInt("PilotSkill", out pilotSkill))
                return;


            if (!objFitFile.GetInt("BuildingName", out turretTypeName))
                turretTypeName = 30164;// IDS_TRTOBJ_NAME;



        }
        #endregion

        #region Public Functions

        public float DamageLevel
        {
            get { return (damageLevel); }

        }

        public override int Appearance
        {
            get { return (int)appearName; }
        }

        #endregion
    }
}