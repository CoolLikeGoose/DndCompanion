using Domain.Entities;
using Domain.ValueObjects;

namespace DndCompanion.Domain.Tests.Entities;

public class MonsterTests
{
    private static Session CreateSession(string? pin = null) =>
        Session.Create(Guid.NewGuid(), "Master", pin is null ? null : PinCode.From(pin));

    public class AddMonster
    {
        [Fact]
        public void Trows_WhenNameIsEmpty()
        {
            var session = CreateSession();
            Assert.Throws<ArgumentException>(() => session.AddMonster("", 10));
        }

        [Fact]
        public void Trows_WhenMaxHPNegative()
        {
            var session = CreateSession();
            Assert.Throws<ArgumentException>(() => session.AddMonster("Test Monster", -1));
        }

        [Fact]
        public void SetsCurrentHP_WhenValid()
        {
            var session = CreateSession();
            var monster = session.AddMonster("Test Monster", 10);
            Assert.Equal(10, monster.CurrentHp);
        }

        [Fact]
        public void TrimsName_WhenValid()
        {
            var session = CreateSession();
            var monster = session.AddMonster("  Test Monster  ", 10);
            Assert.Equal("Test Monster", monster.Name);
        }
    }

    public class AddMonsterWithBattle
    {
        [Fact]
        public void AssignsDefaultBattle_WhenBattleIdNotProvided()
        {
            var session = CreateSession();
            var monster = session.AddMonster("Test Monster", 10);
            Assert.Equal(session.DefaultBattleId, monster.BattleId);
        }

        [Fact]
        public void AssignsGivenBattle_WhenBattleIdProvided()
        {
            var session = CreateSession();
            var battle = session.AddBattle("Boss Fight");
            var monster = session.AddMonster("Test Monster", 10, battleId: battle.BattleId);
            Assert.Equal(battle.BattleId, monster.BattleId);
        }

        [Fact]
        public void Throws_WhenBattleIdNotFoundInSession()
        {
            var session = CreateSession();
            Assert.Throws<ArgumentException>(() =>
                session.AddMonster("Test Monster", 10, battleId: Guid.NewGuid()));
        }
    }
    
    public class MonsterOrdering
    {
        [Fact]
        public void AssignsIncrementalOrder_WhenAddedToSameBattle()
        {
            var session = CreateSession();
            var m1 = session.AddMonster("Monster1", 10);
            var m2 = session.AddMonster("Monster2", 10);
            Assert.True(m2.Order > m1.Order);
        }

        [Fact]
        public void OrderIsIndependent_PerBattle()
        {
            var session = CreateSession();
            var battle = session.AddBattle("Boss Fight");

            var m1 = session.AddMonster("Monster1", 10);
            session.AddMonster("Monster2", 10, battleId: battle.BattleId);
            var m3 = session.AddMonster("Monster3", 10);

            Assert.True(m3.Order > m1.Order);
        }
    }

    public class ReorderMonster
    {
        [Fact]
        public void Throws_WhenMonsterNotFound()
        {
            var session = CreateSession();
            Assert.Throws<ArgumentException>(() => session.ReorderMonster(Guid.NewGuid(), 500));
        }

        [Fact]
        public void SetsOrder_WhenValid()
        {
            var session = CreateSession();
            var monster = session.AddMonster("Monster1", 10);

            session.ReorderMonster(monster.Id, 42.5);

            Assert.Equal(42.5, monster.Order);
        }
    }

    public class MoveMonsterToBattle
    {
        [Fact]
        public void Throws_WhenMonsterNotFound()
        {
            var session = CreateSession();
            var battle = session.AddBattle("Boss Fight");
            Assert.Throws<ArgumentException>(() => session.MoveMonsterToBattle(Guid.NewGuid(), battle.BattleId, 500));
        }

        [Fact]
        public void Throws_WhenTargetBattleNotFound()
        {
            var session = CreateSession();
            var monster = session.AddMonster("Monster1", 10);
            Assert.Throws<ArgumentException>(() => session.MoveMonsterToBattle(monster.Id, Guid.NewGuid(), 500));
        }

        [Fact]
        public void MovesMonsterAndSetsOrder_WhenValid()
        {
            var session = CreateSession();
            var battle = session.AddBattle("Boss Fight");
            var monster = session.AddMonster("Monster1", 10);

            session.MoveMonsterToBattle(monster.Id, battle.BattleId, 250);

            Assert.Equal(battle.BattleId, monster.BattleId);
            Assert.Equal(250, monster.Order);
        }
    }

    public class UpdateMonster
    {
        [Fact]
        public void Throws_WhenMonsterNotFound()
        {
            var session = CreateSession();
            Assert.Throws<ArgumentException>(() => session.UpdateMonster(Guid.NewGuid(), name: "New Name"));
        }

        [Fact]
        public void Throws_WhenNameIsEmptyString()
        {
            var session = CreateSession();
            session.AddMonster("Monster1", 10);
            Assert.Throws<ArgumentException>(() =>
                session.UpdateMonster(session.AddMonster("Monster1", 10).Id, name: ""));
        }

        [Fact]
        public void UpdatesOnlyProvided_WhenValid()
        {
            var session = CreateSession();
            var monster = session.AddMonster("Monster1", 10);
            session.UpdateMonster(monster.Id, name: "Updated Monster");
            Assert.Equal("Updated Monster", monster.Name);
            Assert.Equal(10, monster.CurrentHp);
        }

        [Fact]
        public void Throws_WhenMaxHPNegative()
        {
            var session = CreateSession();
            var monster = session.AddMonster("Monster1", 10);
            Assert.Throws<ArgumentException>(() => session.UpdateMonster(monster.Id, maxHp: -1));
        }

        [Fact]
        public void ClampsCurrentHP_WhenMaxHPLessThanCurrent()
        {
            var session = CreateSession();
            var monster = session.AddMonster("Monster1", 10);
            session.UpdateMonster(monster.Id, maxHp: 5);
            Assert.Equal(5, monster.CurrentHp);
        }

        [Fact]
        public void ClampsToZero_WhenCurrentHPLessThanZero()
        {
            var session = CreateSession();
            var monster = session.AddMonster("Monster1", 10);
            session.UpdateMonster(monster.Id, currentHp: -1);
            Assert.Equal(0, monster.CurrentHp);
        }

        [Fact]
        public void ClampsToMaxHP_WhenCurrentHPGreaterThanMax()
        {
            var session = CreateSession();
            var monster = session.AddMonster("Monster1", 10);
            session.UpdateMonster(monster.Id, currentHp: 15);
            Assert.Equal(10, monster.CurrentHp);
        }

        [Fact]
        public void ChangeName_WhenProvided()
        {
            var session = CreateSession();
            var monster = session.AddMonster("Monster1", 10);
            session.UpdateMonster(monster.Id, name: "Updated Monster", currentHp: 5);
            Assert.Equal("Updated Monster", monster.Name);
            Assert.Equal(5, monster.CurrentHp);
        }
    }

    public class AddBestiaryEntry()
    {
        [Fact]
        public void Trows_WhenNameIsEmpty()
        {
            Assert.Throws<ArgumentException>(() => BestiaryEntry.Create(Guid.NewGuid(), "", 10));
        }

        [Fact]
        public void Trows_WhenMaxHPNegative()
        {
            Assert.Throws<ArgumentException>(() => BestiaryEntry.Create(Guid.NewGuid(), "Test Monster", -1));
        }

        [Fact]
        public void TrimsName_WhenValid()
        {
            var entry = BestiaryEntry.Create(Guid.NewGuid(), "  Test Monster  ", 10);
            Assert.Equal("Test Monster", entry.Name);
        }
    }

    public class UpdateBestiaryEntry
    {
        [Fact]
        public void Throws_WhenNameIsEmptyString()
        {
            var monster = BestiaryEntry.Create(Guid.NewGuid(), "Monster1", 10);
            Assert.Throws<ArgumentException>(() =>
                monster.Update(""));
        }

        [Fact]
        public void UpdatesOnlyProvided_WhenValid()
        {
            var monster = BestiaryEntry.Create(Guid.NewGuid(), "Monster1", 10);
            monster.Update(name: "Updated Monster");
            Assert.Equal("Updated Monster", monster.Name);
            Assert.Equal(10, monster.MaxHp);
            Assert.Null(monster.Description);
        }

        [Fact]
        public void Throws_WhenMaxHPNegative()
        {
            var monster = BestiaryEntry.Create(Guid.NewGuid(), "Monster1", 10);
            Assert.Throws<ArgumentException>(() => monster.Update(maxHp: -1));
        }

        [Fact]
        public void ChangeName_WhenProvided()
        {
            var monster = BestiaryEntry.Create(Guid.NewGuid(), "Monster1", 10);
            monster.Update(name: "Updated Monster");
            Assert.Equal("Updated Monster", monster.Name);
        }
    }
}