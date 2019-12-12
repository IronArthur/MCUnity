using UnityEngine;
using UnityEditor;
using MechCommanderUnity.API;
using System;

namespace MechCommanderUnity.MCG.ObjectTypes
{
    [Serializable]
    public class ExplosionType : ObjectType
    {
        #region Class Variables
        public float damageLevel;
        public int soundEffectId;
        public int lightObjectId;
        public int explosionRadius;
        public float chunkSize;

        public float lightMinMaxRadius;
        public float lightMaxMaxRadius;
        public float lightOutMinRadius;
        public float lightOutMaxRadius;
        //public DWORD lightRGB;
        public float maxIntensity;
        public float minIntensity;
        public float duration;
        public float delayUntilCollidable;

        #endregion

        #region Class Structures
        #endregion

        #region Constructors
        public ExplosionType(FITFile objFitFile) : base(objFitFile)
        {
            objectTypeClass = (int)ObjectTypeClass.EXPLOSION_TYPE;
            objectClass = ObjectClass.EXPLOSION;
            damageLevel = 0f;
            soundEffectId = -1;
            explosionRadius = 0;
            chunkSize = 0f;
            delayUntilCollidable = 0.5f;

            lightMinMaxRadius = 0.0f;
            lightMaxMaxRadius = 0.0f;
            lightOutMinRadius = 0.0f;
            lightOutMaxRadius = 0.0f;
            //lightRGB = 0x00000000;
            maxIntensity = 0.0f;
            minIntensity = 0.0f;
            duration = 0.0f;

            if (!objFitFile.SeekSection("ExplosionData"))
                return;

            objFitFile.GetFloat("DmgLevel", out damageLevel);

            if (!objFitFile.GetInt("SoundEffectId", out soundEffectId))
                return;

            if (!objFitFile.GetInt("LightObjectId", out lightObjectId))
                lightObjectId = -1;

            if (!objFitFile.GetInt("ExplosionRadius", out explosionRadius))
                explosionRadius = 0;


            if (!objFitFile.GetFloat("DamageChunkSize", out chunkSize))
                chunkSize = 5f;


            //TODO: objFitFile.SeekSection("LightData"); NOT EXISTS IN MCG?¿?
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