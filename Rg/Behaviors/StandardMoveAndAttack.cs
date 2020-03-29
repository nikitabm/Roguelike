using RogueSharp;
using System;
using System.Linq;
using Rg.Core;
using Rg.Interfaces;

namespace Rg.Behaviors
{
    public class StandardMoveAndAttack : IBehavior
    {
        public bool Act(Actor actor, CommandSystem commandSystem)
        {
            DungeonMap dungeonMap = Game.DungeonMap;
            Player player = Game.Player;
            FieldOfView actorFov = new FieldOfView(dungeonMap);

            // If the monster has not been alerted, compute a field-of-view 
            // Use the monster's Awareness value for the distance in the FoV check
            // If the player is in the monster's FoV then alert it
            // Add a message to the MessageLog regarding this alerted status
            if (!actor.TurnsAlerted.HasValue)
            {
                actorFov.ComputeFov(actor.X, actor.Y, actor.Awareness, true);
                if (actorFov.IsInFov(player.X, player.Y))
                {
                    Game.MessageLog.Add($"{actor.Name} is eager to fight {player.Name}");
                    actor.TurnsAlerted = 1;
                }
            }
            if (actor.TurnsAlerted.HasValue)
            {
                // Before we find a path, make sure to make the monster and player Cells walkable
                dungeonMap.SetIsWalkable(actor.X, actor.Y, true);
                dungeonMap.SetIsWalkable(player.X, player.Y, true);

                PathFinder pathFinder = new PathFinder(dungeonMap);
                Path path = null;

                try
                {
                    path = pathFinder.ShortestPath(
                    dungeonMap.GetCell(actor.X, actor.Y),
                    dungeonMap.GetCell(player.X, player.Y));
                }
                catch (PathNotFoundException)
                {
                    // The monster can see the player, but cannot find a path to him
                    // This could be due to other monsters blocking the way
                    // Add a message to the message log that the monster is waiting
                    Game.MessageLog.Add($"{actor.Name} waits for a turn");
                }

                // Don't forget to set the walkable status back to false
                dungeonMap.SetIsWalkable(actor.X, actor.Y, false);
                dungeonMap.SetIsWalkable(player.X, player.Y, false);

                // In the case that there was a path, tell the CommandSystem to move the monster
                if (path != null)
                {
                    try
                    {
                        commandSystem.MoveActor(actor, path.StepForward());
                    }
                    catch (NoMoreStepsException)
                    {
                        Game.MessageLog.Add($"{actor.Name} growls in frustration");
                    }
                }

                actor.TurnsAlerted++;

                // Lose alerted status every 15 turns. 
                // As long as the player is still in FoV the monster will stay alert
                // Otherwise the monster will quit chasing the player.
                if (actor.TurnsAlerted > 15)
                {
                    actor.TurnsAlerted = null;
                }
            }
            return true;
        }
    }
}
