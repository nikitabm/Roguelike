using RLNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rg.Core
{
    public class Player : Actor
    {
        public Player()
        {
            Level = 1;
            Exp = 0;
            Attack = 2;
            AttackChance = 50 + Level * 2;
            Awareness = 10 + Level;
            Color = Colors.Player;
            Defense = 2;
            DefenseChance = 40 + Level;
            Gold = 0;
            Health = 100;
            MaxHealth = 100;
            Name = "Lucy";
            Speed = 10 + Level;
            Symbol = '@';


        }
        public void GainExp()
        {
            Exp++;
            if (Exp == 4)
            {
                Level++;
                Exp = 0;
            }
        }
        public void DrawStats(RLConsole statConsole)
        {
            statConsole.Print(1, 1, $"Name:     {Name}", Colors.Text);
            statConsole.Print(1, 3, $"Level:    {Level}", Colors.Text);
            statConsole.Print(1, 5, $"Exp:      {Exp}/4", Colors.Text);
            statConsole.Print(1, 7, $"Health:   {Health}/{MaxHealth}", Colors.Text);
            statConsole.Print(1, 9, $"Attack:   {Attack} ({AttackChance}%)", Colors.Text);
            statConsole.Print(1, 11, $"Defense:  {Defense} ({DefenseChance}%)", Colors.Text);
        }
    }
}
