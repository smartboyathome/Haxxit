using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartboyDevelopments.Haxxit.Commands;

namespace SmartboyDevelopments.Haxxit.Programs
{
    public abstract class ProgramFactory : IEquatable<ProgramFactory>, IFactory<Program>
    {
        public ushort Moves
        {
            get;
            protected set;
        }
        public ushort Size
        {
            get;
            protected set;
        }
        public ushort SpawnWeight
        {
            get;
            protected set;
        }
        public string TypeName
        {
            get;
            protected set;
        }
        public IEnumerable<Command> Commands
        {
            get;
            protected set;
        }

        public virtual Program NewInstance()
        {
            return new Program(Moves, Size, TypeName, Commands);
        }

        public virtual bool Equals(ProgramFactory other)
        {
            if (Moves != other.Moves || Size != other.Size || TypeName != other.TypeName || Commands.Count() != other.Commands.Count())
                return false;
            for (int i = 0; i < Commands.Count(); i++)
                if (Commands.ElementAt(i) != other.Commands.ElementAt(i))
                    return false;
            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                IEnumerator<int> primes = Primes.sieve().GetEnumerator();
                int hash = TypeName.GetHashCode();
                hash = hash * primes.Current + Moves.GetHashCode();
                primes.MoveNext();
                hash = hash * primes.Current + Size.GetHashCode();
                primes.MoveNext();
                hash = hash * primes.Current + SpawnWeight.GetHashCode();
                primes.MoveNext();
                foreach (Command command in Commands)
                {
                    hash = hash * primes.Current + command.GetHashCode();
                    primes.MoveNext();
                }
                return hash;
            }
        }
    }
}
