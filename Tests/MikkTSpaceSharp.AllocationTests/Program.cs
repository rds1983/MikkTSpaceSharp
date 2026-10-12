using System;
using System.Threading.Tasks;
using Hebron.Runtime;
using MikkTSpaceSharp;

internal static class Program
{
    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    private static MikkTSpace.SMikkTSpaceContext Context(int failingPhase = 0)
    {
        return new MikkTSpace.SMikkTSpaceContext
        {
            m_getNumFaces = () => 1,
            m_getNumVerticesOfFace = _ => 3,
            m_getPosition = (_, corner) =>
            {
                if (failingPhase == 1) throw new InvalidOperationException("position");
                return new SVec3(corner == 1 ? 1 : 0, corner == 2 ? 1 : 0, 0);
            },
            m_getNormal = (_, _) =>
            {
                if (failingPhase == 2) throw new InvalidOperationException("normal");
                return new SVec3(0, 0, 1);
            },
            m_getTexCoord = (_, corner) =>
            {
                if (failingPhase == 3) throw new InvalidOperationException("UV");
                return new SVec2(corner == 1 ? 1 : 0, corner == 2 ? 1 : 0);
            },
            m_setTSpaceBasic = (_, _, _, _) =>
            {
                if (failingPhase == 4) throw new InvalidOperationException("output");
            }
        };
    }

    private static void Failure(int phase)
    {
        var before = MikkTSpaceSharpMemoryStats.Allocations;
        var threw = false;
        try { MikkTSpace.genTangSpaceDefault(Context(phase)); }
        catch (InvalidOperationException) { threw = true; }
        Assert(threw, "The injected callback must throw.");
        Assert(MikkTSpaceSharpMemoryStats.Allocations == before, "Callback exception leaked a temporary block.");
    }

    private static unsafe void Main()
    {
        var baseline = MikkTSpaceSharpMemoryStats.Allocations;
        for (var phase = 1; phase <= 4; phase++)
            for (var repeat = 0; repeat < 20; repeat++) Failure(phase);
        for (var repeat = 0; repeat < 20; repeat++) Assert(MikkTSpace.genTangSpaceDefault(Context()), "Ordinary generation failed.");
        Assert(MikkTSpaceSharpMemoryStats.Allocations == baseline, "Ordinary generation leaked.");
        var nested = Context(); var get = nested.m_getPosition; var once = false;
        nested.m_getPosition = (face, corner) =>
        {
            if (!once) { once = true; Failure(4); }
            return get(face, corner);
        };
        Assert(MikkTSpace.genTangSpaceDefault(nested), "Nested generation failed.");
        Assert(MikkTSpaceSharpMemoryStats.Allocations == baseline, "Nested scopes lost ownership.");
        Parallel.For(0, 40, _ => Assert(MikkTSpace.genTangSpaceDefault(Context()), "Concurrent generation failed."));
        Assert(MikkTSpaceSharpMemoryStats.Allocations == baseline, "Concurrent scopes lost ownership.");
        var overflow = false;
        try { CRuntime.malloc(ulong.MaxValue); }
        catch (OverflowException) { overflow = true; }
        Assert(overflow && MikkTSpaceSharpMemoryStats.Allocations == baseline, "Overflowed size must not allocate.");
        Console.WriteLine("80 callback failures, 20 ordinary calls, nested and 40 concurrent calls: temporary allocation balance preserved.");
    }
}
