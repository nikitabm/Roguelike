using System;
using Rg.Core;
namespace Rg.Interfaces
{
    public interface IBehavior
    {
        bool Act(Actor actor, CommandSystem commandSystem);
    }
}
