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