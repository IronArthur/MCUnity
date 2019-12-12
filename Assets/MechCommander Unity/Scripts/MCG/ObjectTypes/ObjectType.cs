using MechCommanderUnity.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace MechCommanderUnity.MCG
{
    [Serializable]
    public class ObjectType
    {
        #region Class Variables

        public int objTypeNum;              //What exactly am I?
        public int numUsers;              //How many people love me?
        public int objectTypeClass;       //What type am I?
        public ObjectClass objectClass;            //What object class am i?
        public int DestroyedObject;      //What I turn into when I die.
        public int ExplosionObject;       //How I blow up
        public bool potentialContact;      //Can I can be a contact?
        public string name;
        public int appearName;           //Base Name of appearance Files.
        public float extentRadius;         //Smallest sphere which will hold me.
        public bool keepMe;                    //Do not EVER cache this objType out.
        public int iconNumber;                //my index into the big strip o' icons
        public int teamId;                    //DEfault for this type
        public int subType;               //if building, what type of building? etc.


        #endregion



        #region Class Structures

        

        #endregion

        #region Constructors

        public ObjectType()
        {
            objectClass = ObjectClass.INVALID;
            objectTypeClass = -1;           //This is an invalid_object
            DestroyedObject = -1;
            ExplosionObject = -1;
            potentialContact = false;

            extentRadius = 0;               //Nothing can hit me if this is zero.

            keepMe = false;

            iconNumber = -1;                //defaults to no icon

            appearName = 0;
            subType = 0;
        }

        public ObjectType(int objTypeNum)
        {
            this.objTypeNum = objTypeNum;
        }

        public ObjectType(FITFile objFitFile)
        {

            objFitFile.SeekSection("ObjectType");
            objFitFile.GetString("Name", out name);


            objFitFile.GetInt("Appearance", out appearName);
            objFitFile.GetInt("ExplosionObject", out int obj);
            ExplosionObject = obj;
            objFitFile.GetInt("DestroyedObject", out obj);
            DestroyedObject = obj;
            objFitFile.GetFloat("ExtentRadius", out extentRadius);



            keepMe = true;                  //Never cache out anymore!
            teamId = -1;		//Everything starts out Neutral now.

        }

        #endregion

        #region Public Functions



        public void NoCacheOut()
        {
            this.keepMe = true;
        }

        
        public virtual int Appearance
        {
            get { return (int)appearName;  }
        }

        public virtual int AppearanceType
        {
            get { return (int)appearName << 24; }
        }
        public virtual int AppearanceName
        {
            get { return (int)appearName & 0xFFFFFF; }
        }

        public string Name
        {
            get { return name; }
        }
        #endregion
    }
    
}
