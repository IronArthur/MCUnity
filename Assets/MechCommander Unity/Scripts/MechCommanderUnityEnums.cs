using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MechCommanderUnity
{

    public enum SpecialArea
    {
        SPECIAL_NONE = 0,
        SPECIAL_WALL = 1,
        SPECIAL_GATE = 2,
        SPECIAL_LAND_BRIDGE = 3,
        SPECIAL_FOREST = 4
    }

    public enum AreaType
    {
        AREA_TYPE_NORMAL,
        AREA_TYPE_WALL,
        AREA_TYPE_GATE,
        AREA_TYPE_LAND_BRIDGE,
        AREA_TYPE_FOREST,
        NUM_AREA_TYPES
    }
    public enum WaterType
    {
        WATER_TYPE_NONE,
        WATER_TYPE_SHALLOW,
        WATER_TYPE_DEEP,
        NUM_WATER_TYPES
    }

    public enum AppearanceClass
    {
        BASE_APPEARANCE = 0,
        SPRITE_TREE = 1,
        VFX_APPEAR = 2,
        FSY_APPEAR = 3, //NOT USED
        LINE_APPEAR = 4,
        GV_TYPE = 5,
        ARM_APPEAR = 6,
        BUILD_APPEAR = 7,
        ELM_TREE = 8,
        PU_TYPE = 9,
        //SMOKE_TYPE = 0x0a,
        //POLY_APPEARANCE = 0x0b,
        //MLR_APPEARANCE = 0x0c,
        //MECH_TYPE = 0x0d,
        //BLDG_TYPE = 0x0e,
        //TREED_TYPE = 0x0f,

        //BUILDING_APPR_TYPE = 0x10,
        //TREE_APPR_TYPE = 0x11,
        //VEHICLE_APPR_TYPE = 0x12,
        //MECH_APPR_TYPE = 0x13,
        //GENERIC_APPR_TYPE = 0x14
    }

    


    public enum ObjectStatusType
    {
        OBJECT_STATUS_NORMAL,
        OBJECT_STATUS_DISABLED,
        OBJECT_STATUS_DESTROYED,
        OBJECT_STATUS_STARTING_UP,
        OBJECT_STATUS_SHUTTING_DOWN,
        OBJECT_STATUS_SHUTDOWN,
        NUM_OBJECT_STATUSES
    };


    public enum MissionLoadType
    {
        MISSION_LOAD_SP_QUICKSTART,
        MISSION_LOAD_SP_LOGISTICS,
        MISSION_LOAD_MP_QUICKSTART,
        MISSION_LOAD_MP_LOGISTICS
    }

    public enum ObjectTypeClass
    {
        /*
        {0 , "Objetos/Tree"},
        {1 , "Buildings"},
        {2 , "Mech"},
        {3 , "Vehicle"},
        {4 , "Explosion"},
        {5 , "Building Explosion"},
        {6 , "Weapons FX"},
        {7 , "Smoke"},
        {8 , "Projectiles"},
        {9 , "Mech Fallen Arms"},
        {10 , "Waypoint/Map/Artillery Indicator"},
        {11 , "Template/Misc Terrain Object"},
        {12 , "Artillery Indicator"},
        {13 , "Mines"},
        {14 , "ElementalBase Data File"},
        {15 , "Misc Terrain Object"},
        {16 , "Jump FX"},
        {17 , "Weapon Projectiles"},
        {18 , "Terrain Files Various"},
        {19 , "Camera Drone"},
        {20 , "Train"},
        {21 , "Turrets"},
        {22 , "Gates"},
        {23 , "Lights Objects"}
         */
        CRAPPY_OBJECT = -1,
        TREE_TYPE,
        BUILDING_TYPE,
        BATTLEMECH_TYPE,
        VEHICLE_TYPE,
        EXPLOSION_TYPE, //was EXPLODER_TYPE
        FIRE_TYPE,
        LASER_TYPE,
        SMOKES_TYPE,
        PROJECTILE_TYPE,
        MECHARM_TYPE,
        MAPICON_TYPE,
        TERRAINOBJECT_TYPE,
        ARTILLERY_TYPE,
        MINE_TYPE,
        ELEMENTAL_TYPE,
        BRIDGE_TYPE,
        JET_TYPE,
        PROJLASER_TYPE,
        TREEBUILDING_TYPE,
        CAMERADRONE_TYPE,
        TRAINCAR_TYPE,
        TURRET_TYPE,
        GATE_TYPE,
        KLIEG_LIGHT_TYPE,
        WEAPONBOLT_TYPE
    };
    public enum ObjectClass
    {
        INVALID = -1,
        BASEOBJECT = 0,
        TERRN,
        BATTLEMECH,
        GROUNDVEHICLE,
        ELEMENTAL,
        EXPLOSION,   //was EXPLODE
        FIRE,
        ARTILLERY,
        MOVER,
        GAMEOBJECT,
        BIGGAMEOBJECT,
        COMPONENT,
        WEAPON,
        PROJECTILE,
        LASERWEAPON,
        PPC,
        BUILDING,
        SMOKE,
        BULLET,
        DEBRIS,
        MAP_ICON,
        TREE,
        TERRAINOBJECT,
        MINE,
        BRIDGE,
        JET,
        PROJLASER,
        TREEBUILDING,
        CAMERADRONE,
        TRAINCAR,
        TURRET,
        GATE,
        KLIEG_LIGHT,
        WEAPONBOLT
    };


    public enum ActorStates
    {
        STATE_INVALID = -1,
        STATE_NORMAL = 0,
        STATE_BLOWING_UP1,        //From Normal to Damaged 
        STATE_DAMAGED,
        STATE_BLOWING_UP2,        //From Damaged to Destroyed 
        STATE_DESTROYED,
        STATE_FALLEN_DMG,         //Used for Trees
        MAX_ACTOR_STATES
    };
    
    [Serializable]
    public struct ActorData
    {
        public ActorStates state;
        public int subState;
        public int symmetrical;                  // are second-half rotations flip versions of first half?
        public int numRotations;                 // number of rotations (including flips)
        public int numFrames;                        // number of frames for this gesture (if -1, does not exist)
        public int basePacketNumber;             // Where in packet file does this gesture start.
        public float frameRate;                        // intended frame rate of playback
        public int delta;                        //First Image is Shadow/Floor?¿?
        public int shapeId;
    };
    
    public enum GVActorStates
    {
        STATE_INVALID = -1,
        STATE_STOPPED = 0,
        STATE_MOVING,        //From Normal to Damaged 
        STATE_DESTROYED
    };
    

    [Serializable]
    public struct GVActorData
    {
        public GVActorStates state;
        public int symmetrical;                  // are second-half rotations flip versions of first half?
        public int numRotations;                 // number of rotations (including flips)
        public int numFrames;                        // number of frames for this gesture (if -1, does not exist)
        public int basePacketNumber;             // Where in packet file does this gesture start.
        public float frameRate;                        // intended frame rate of playback
        public int shapeId;
    };


}
