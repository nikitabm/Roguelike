using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Interfaces;
using RLNET;
using RogueSharp;

namespace Rg.Core
{
    public class Item : IDrawable
    {
        // IDrawable
        public RLColor Color { get; set; }
        public char Symbol { get; set; }
        public int Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public void Draw(RLConsole console, IMap map)
        {
            if (!map.GetCell(X, Y).IsExplored)
            {
                return;
            }
            if (map.IsInFov(X, Y))
            {
                Color = Colors.Player;
            }
            else
            {
                Color = Colors.Floor;
            }

            console.Set(X, Y, Color, null, Symbol);
        }
    }
}
