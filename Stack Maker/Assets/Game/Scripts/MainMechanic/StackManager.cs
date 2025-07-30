using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StackManager : Singleton<StackManager>
{
    [SerializeField] private Transform playerVisual, rootPos;
    [SerializeField] private BoxCollider stackTileCollider;
    private float DeltaPosY => stackTileCollider.size.y;
    [SerializeField] private List<GameObject> tiles;

    private void Start()
    {
        //display brick what player  bring
        CanvasController.Instance.UpdateStackIndicatorText(tiles.Count);
    }

    public void CollectTile(GameObject newTile)
    {
        AddTile(newTile);
        RefreshElementPos(playerVisual.transform, false);     
    }

    //add brick into stack
    private void AddTile(GameObject newTile)
    {
        newTile.tag = "Untagged";

        var peakTile = tiles.Last().transform;

        var newTilePos = peakTile.position;
        newTilePos.y += DeltaPosY;
        newTile.transform.position = newTilePos;
        newTile.transform.SetParent(rootPos);
        tiles.Add(newTile);

        // update UI the player is bring
        CanvasController.Instance.UpdateStackIndicatorText(tiles.Count);
    }

    //Remove Tile and place brick on the bridge
    public bool RemoveTile(Vector3 tileNewPos)
    {
        if(tiles.Count > 1)
        {
            // take brick first
            var tileToBeRemove = tiles.First();

            tileToBeRemove.tag = "Untagged";// change tag 

            tileToBeRemove.transform.SetParent(null);

            //deleteRemove = place of bridge
            tileToBeRemove.transform.position = tileNewPos;
            tileToBeRemove.isStatic = true;
            tiles.RemoveAt(0);
            // after remove the brick, down brick and player
            foreach (var tile in tiles)
            {
                RefreshElementPos(tile.transform, true);
            }
            RefreshElementPos(playerVisual.transform, true);
            // update UI the score is deleted
            CanvasController.Instance.UpdateStackIndicatorText(tiles.Count);

            return true;
        }
        else
        {
            return false;
        }
    }

    //up and low
    private void RefreshElementPos(Transform element, bool moveDown)
    {
        var newPos = element.position;

        if (moveDown)
            newPos.y -= DeltaPosY;
        else
            newPos.y += DeltaPosY;            

        element.position = newPos;
    }
}
