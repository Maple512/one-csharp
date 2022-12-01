namespace OneI.Logable.Enrichers;

using System.Collections;
using System.Collections.Generic;

internal class EnricherStack : IEnumerable<ILoggerEnricher>
{
    private readonly EnricherStack? _under;
    private readonly ILoggerEnricher? _top;

    private EnricherStack() { }

    private EnricherStack(EnricherStack? under, ILoggerEnricher? top)
    {
        _under = CheckTools.NotNull(under);
        _top = top;

        Count = _under.Count + 1;
    }

    public static EnricherStack Empty => new();

    IEnumerator<ILoggerEnricher> IEnumerable<ILoggerEnricher>.GetEnumerator() => new Enumerator(this);

    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    public Enumerator GetEnumerator() => new(this);

    public int Count { get; }

    public bool IsEmpty => _under == null;

    public EnricherStack Push(ILoggerEnricher enricher) => new(this, enricher);

    public ILoggerEnricher? Top => _top;

    internal struct Enumerator : IEnumerator<ILoggerEnricher>
    {
        private readonly EnricherStack _stack;
        private EnricherStack _top;
        private ILoggerEnricher? _current;

        public Enumerator(EnricherStack stack)
        {
            _stack = stack;
            _top = stack;
            _current = null;
        }

        public ILoggerEnricher Current => _current!;

        object IEnumerator.Current => _current!;

        public void Dispose() { }

        public bool MoveNext()
        {
            if(_top.IsEmpty)
            {
                return false;
            }

            _current = _top.Top;
            _top = _top._under!;

            return true;
        }

        public void Reset()
        {
            _top = _stack;
            _current = null;
        }
    }
}
