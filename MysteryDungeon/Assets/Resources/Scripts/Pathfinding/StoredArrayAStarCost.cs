﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoredArrayAStarCost : AStarCost {

    BoardCreator _boardCreator;
    TileType[][] tileArray;

    public StoredArrayAStarCost(BoardCreator boardCreator) {
        _boardCreator = boardCreator;
        tileArray = boardCreator.GetTileArray();
    }

    public override float getCost(int toX, int toY, int fromX, int fromY) {

        if (toX != fromX && toY != fromY) {
            //Diagonal, so check if can move in both orthogonal directions.
            if (isPassable(toX, fromY, fromX, fromY) && isPassable(fromX, toY, fromX, fromY)) {
                return SpaceConstants.GRID_DIAG;
            }
        } else if (isPassable(toX, toY, fromX, fromY)) {
            return 1;
        }
        return -1;
    }

    private bool isPassable(int toX, int toY, int fromX, int fromY) {

        //Check bounds
        if(toX < 0 || fromX < 0 || toX >= _boardCreator.rows || fromX >= _boardCreator.rows) {
            return false;
        }else if (toY < 0 || fromY < 0 || toY >= _boardCreator.columns || fromY >= _boardCreator.columns) {
            return false;
        }

        if(tileArray[fromX][fromY] != TileType.Wall && tileArray[toX][toY] != TileType.Wall) {
            return true;
        }

        return false;
    }
}
