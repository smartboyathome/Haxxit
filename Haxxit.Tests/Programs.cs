using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartboyDevelopments.Haxxit.Tests
{
    class BasicProgram : Program
    {
        public BasicProgram()
        {
            _moves = new Movement(4);
            _size = new ProgramSize(4);
            _commands.Add("Damage", new BasicDamageCommand());
        }
    }

    class FasterBasicProgram : Program
    {
        public FasterBasicProgram()
        {
            _moves = new Movement(8);
            _size = new ProgramSize(4);
            _commands.Add("Damage", new BasicDamageCommand());
        }
    }

    class BiggerFasterBasicProgram : Program
    {
        public BiggerFasterBasicProgram()
        {
            _moves = new Movement(16);
            _size = new ProgramSize(8);
            _commands.Add("Damage", new BasicDamageCommand());
        }
    }
}
