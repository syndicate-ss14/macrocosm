using Content.Shared.TicketPrinter;
using Content.Server.Stack;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;

namespace Content.Server._MACRO.TicketPrinter;

public sealed class TicketPrinterSystem : SharedTicketPrinterSystem
{
    [Dependency] private readonly StackSystem _stack = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;

    public override void Initialize()
    {
        base.Initialize();
    }

    /// <summary>
    /// Applies ticket multiplier and spawns tickets, stores any remainder for future spawns
    /// </summary>
    /// <param name="ent">Entity spawning the tickets</param>
    /// <param name="amount">Base amount of tickets to spawn</param>
    protected override void PrintTickets(Entity<TicketPrinterComponent> ent, float amount)
    {
        if (!_cfg.GetCVar(CCVars.SalvageTicketsEnabled))
            return;

        var proto = ent.Comp.TicketProtoId.ToString();
        var spawnAmount = ent.Comp.Remainder + amount * ent.Comp.TicketMultiplier;
        if (spawnAmount <= 0 || proto == string.Empty)
            return;

        var tickets = _stack.SpawnMultipleAtPosition(ent.Comp.TicketProtoId, (int)Math.Floor(spawnAmount), Transform(ent).Coordinates);
        foreach (var ticket in tickets)
            _stack.TryMergeToContacts(ticket); //try to make into a single stack

        ent.Comp.Remainder = spawnAmount - (float)Math.Floor(spawnAmount); //can't spawn fractional tickets so store for the future
    }
}