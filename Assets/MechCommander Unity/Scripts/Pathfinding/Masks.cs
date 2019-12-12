using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Masks
{

    const int MAPCELL_TERRAIN_SHIFT = 0;
    const int MAPCELL_TERRAIN_MASK = 0x0000000F;

    const int MAPCELL_OVERLAY_SHIFT = 4;
    const int MAPCELL_OVERLAY_MASK = 0x00000030;

    const int MAPCELL_MOVER_SHIFT = 6;
    const int MAPCELL_MOVER_MASK = 0x00000040;

    const int MAPCELL_UNUSED1_SHIFT = 7;            // THIS BIT AVAILABLE!
    const int MAPCELL_UNUSED1_MASK = 0x00000080;

    const int MAPCELL_GATE_SHIFT = 8;
    const int MAPCELL_GATE_MASK = 0x00000100;

    const int MAPCELL_OFFMAP_SHIFT = 9;
    const int MAPCELL_OFFMAP_MASK = 0x00000200;

    const int MAPCELL_PASSABLE_SHIFT = 10;
    const int MAPCELL_PASSABLE_MASK = 0x00000400;

    const int MAPCELL_PATHLOCK_SHIFT = 11;
    const int MAPCELL_PATHLOCK_MASK = 0x00001800;
    const int MAPCELL_PATHLOCK_BASE = 0x00000800;

    const int MAPCELL_MINE_SHIFT = 13;
    const int MAPCELL_MINE_MASK = 0x0001E000;

    const int MAPCELL_PRESERVED_SHIFT = 17;
    const int MAPCELL_PRESERVED_MASK = 0x00020000;

    const int MAPCELL_HEIGHT_SHIFT = 18;
    const int MAPCELL_HEIGHT_MASK = 0x003C0000;

    const int MAPCELL_DEBUG_SHIFT = 22;
    const int MAPCELL_DEBUG_MASK = 0x00C00000;

    const int MAPCELL_WALL_SHIFT = 24;
    const int MAPCELL_WALL_MASK = 0x01000000;

    const int MAPCELL_ROAD_SHIFT = 25;
    const int MAPCELL_ROAD_MASK = 0x02000000;

    const int MAPCELL_SHALLOW_SHIFT = 26;
    const int MAPCELL_SHALLOW_MASK = 0x04000000;

    const int MAPCELL_DEEP_SHIFT = 27;
    const int MAPCELL_DEEP_MASK = 0x08000000;

    const int MAPCELL_FOREST_SHIFT = 28;
    const int MAPCELL_FOREST_MASK = 0x10000000;

    //------------------------------------------------------
    // The following are used ONLY when building map data...
    const int MAPCELL_BUILD_WALL_SHIFT = 29;
    const int MAPCELL_BUILD_WALL_MASK = 0x20000000;

    const int MAPCELL_BUILD_GATE_SHIFT = 30;
    const int MAPCELL_BUILD_GATE_MASK = 0x40000000;

    const int MAPCELL_BUILD_LAND_BRIDGE_SHIFT = 11;
    const int MAPCELL_BUILD_LAND_BRIDGE_MASK = 0x00000800;

    const int MAPCELL_BUILD_SPECIAL_MASK = 0x60000800;

    //  const int MAPCELL_BUILD_NOT_SET_SHIFT = 31;
    // const int MAPCELL_BUILD_NOT_SET_MASK = 0x80000000;


    public static long getTerrain(long data)
    {
        Debug.Log((data & MAPCELL_TERRAIN_MASK));

        return ((data & MAPCELL_TERRAIN_MASK) >> MAPCELL_TERRAIN_SHIFT);
    }


    public static long getOverlay(long data)
    {
        return ((data & MAPCELL_OVERLAY_MASK) >> MAPCELL_OVERLAY_SHIFT);
    }

    public static bool getMover(long data)
    {
        return (((data & MAPCELL_MOVER_MASK) >> MAPCELL_MOVER_SHIFT) == 1 ? true : false);
    }

    public static long getGate(long data)
    {
        return ((data & MAPCELL_GATE_MASK) >> MAPCELL_GATE_SHIFT);
    }

    public static bool getPassable(long data)
    {
     //   Debug.Log((data & MAPCELL_PASSABLE_MASK) >> MAPCELL_PASSABLE_SHIFT);
      //  Debug.Log((data & MAPCELL_PASSABLE_MASK));
        return (((data & MAPCELL_PASSABLE_MASK) >> MAPCELL_PASSABLE_SHIFT) == 1 ? true : false);
    }

    public static long getLocalHeight(long data)
    {
        Debug.Log((data & MAPCELL_HEIGHT_MASK));
        return ((data & MAPCELL_HEIGHT_MASK) >> MAPCELL_HEIGHT_SHIFT);
    }
}

