namespace LinqUsageAnalyzer.Interfaces
{
    public class LinqOperator
    {
        public LinqKind LinqKind { get; private set; }
        public string Name { get; private set; }

        public LinqOperator(LinqKind linqKind, string name)
        {
            LinqKind = linqKind;
            Name = name;
        }

        protected bool Equals(LinqOperator other)
        {
            return LinqKind == other.LinqKind && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LinqOperator) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) LinqKind*397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return $"{nameof(LinqKind)}: {LinqKind}, {nameof(Name)}: {Name}";
        }
    }
}