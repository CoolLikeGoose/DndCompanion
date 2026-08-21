using Domain.Entities;
using Domain.ValueObjects;

namespace DndCompanion.Domain.Tests.Entities;

public class BattleTests
{
    private static Session CreateSession(string? pin = null) =>
        Session.Create(Guid.NewGuid(), "Master", pin is null ? null : PinCode.From(pin));

    public class AddBattle
    {
        [Fact]
        public void CreatesDefaultBattle_WhenSessionCreated()
        {
            var session = CreateSession();
            Assert.Single(session.Battles);
            Assert.Equal(session.DefaultBattleId, session.Battles.Single().BattleId);
        }

        [Fact]
        public void Trows_WhenNameIsEmpty()
        {
            var session = CreateSession();
            Assert.Throws<ArgumentException>(() => session.AddBattle(""));
        }

        [Fact]
        public void TrimsName_WhenValid()
        {
            var session = CreateSession();
            var battle = session.AddBattle("  Boss Fight  ");
            Assert.Equal("Boss Fight", battle.Name);
        }

        [Fact]
        public void AddsToSessionBattles_WhenValid()
        {
            var session = CreateSession();
            var battle = session.AddBattle("Boss Fight");
            Assert.Contains(session.Battles, b => b.BattleId == battle.BattleId);
        }
    }
    
    public class BattleOrdering
    {
        [Fact]
        public void AssignsIncrementalOrder_WhenAdded()
        {
            var session = CreateSession();
            var b1 = session.AddBattle("Battle1");
            var b2 = session.AddBattle("Battle2");
            Assert.True(b2.Order > b1.Order);
        }
    }

    public class ReorderBattle
    {
        [Fact]
        public void Throws_WhenBattleNotFound()
        {
            var session = CreateSession();
            Assert.Throws<ArgumentException>(() => session.ReorderBattle(Guid.NewGuid(), 500));
        }

        [Fact]
        public void SetsOrder_WhenValid()
        {
            var session = CreateSession();
            var battle = session.AddBattle("Boss Fight");

            session.ReorderBattle(battle.BattleId, 750);

            Assert.Equal(750, battle.Order);
        }
    }

    public class RemoveBattle
    {
        [Fact]
        public void Throws_WhenBattleNotFound()
        {
            var session = CreateSession();
            Assert.Throws<ArgumentException>(() => session.RemoveBattle(Guid.NewGuid()));
        }

        [Fact]
        public void Throws_WhenRemovingDefaultBattle()
        {
            var session = CreateSession();
            Assert.Throws<InvalidOperationException>(() => session.RemoveBattle(session.DefaultBattleId));
        }

        [Fact]
        public void RemovesBattle_WhenValid()
        {
            var session = CreateSession();
            var battle = session.AddBattle("Boss Fight");
            session.RemoveBattle(battle.BattleId);
            Assert.DoesNotContain(session.Battles, b => b.BattleId == battle.BattleId);
        }
    }

    public class Rename
    {
        [Fact]
        public void Trows_WhenNameIsEmpty()
        {
            var session = CreateSession();
            var battle = session.AddBattle("Boss Fight");
            Assert.Throws<ArgumentException>(() => battle.Rename(""));
        }

        [Fact]
        public void TrimsName_WhenValid()
        {
            var session = CreateSession();
            var battle = session.AddBattle("Boss Fight");
            battle.Rename("  Final Boss  ");
            Assert.Equal("Final Boss", battle.Name);
        }
    }
}