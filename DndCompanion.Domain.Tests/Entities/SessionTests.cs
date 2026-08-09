using Domain.Entities;
using Domain.ValueObjects;

namespace DndCompanion.Domain.Tests.Entities;

public class SessionTests
{
    private static Session CreateSession(string? pin = null) => 
        Session.Create(Guid.NewGuid(), "Master", pin is null ? null : PinCode.From(pin));
    
    public class Create
    {
        [Fact]
        public void CreatesSession_WithMasterParticipant()
        {
            var session = CreateSession();
            Assert.Single(session.Participants);
        }

        [Fact]
        public void Throws_WhenMasterDisplayNameEmpty()
        {
            Assert.Throws<ArgumentException>(() => 
                Session.Create(Guid.NewGuid(), "", null));
        }
    }
    
    public class Join
    {
        [Fact]
        public void AddsParticipants_WhenValid()
        {
            var session = CreateSession();
            session.Join("Player1");
            Assert.Equal(2, session.Participants.Count);
        }
        
        [Fact]
        public void Throws_WhenPinCodeRequiredAndNotProvided()
        {
            var session = CreateSession("1234");
            Assert.Throws<ArgumentException>(() => session.Join("Player1"));
        }

        [Fact]
        public void Throws_WhenPinCodeInvalid()
        {
            var session = CreateSession("1234");
            Assert.Throws<ArgumentException>(() => 
                session.Join("Player1", pinCode: PinCode.From("123")));
        }
        
        [Fact]
        public void Joins_WhenPinCodeValid()
        {
            var session = CreateSession("1234");
            session.Join("Player1", pinCode: PinCode.From("1234"));
            Assert.Equal(2, session.Participants.Count);
        }
        
        [Fact]
        public void Throws_WhenUserAlreadyJoined()
        {
            var session = CreateSession();
            var userId = Guid.NewGuid();
            session.Join("Player1", userId);
            Assert.Throws<ArgumentException>(() => session.Join("Player1", userId));
        }
    }
    
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
        
        [Fact]
        public void Throws_WhenMonsterAlreadyExists()
        {
            var session = CreateSession();
            session.AddMonster("Test Monster", 10);
            Assert.Throws<ArgumentException>(() => session.AddMonster("Test Monster", 10));
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
        public void Throws_WhenNameAlreadyExists()
        {
            var session = CreateSession();
            var monster1 = session.AddMonster("Monster1", 10);
            var monster2 = session.AddMonster("Monster2", 10);
            Assert.Throws<ArgumentException>(() => session.UpdateMonster(monster2.Id, name: "Monster1"));
        }

        [Fact]
        public void Throws_WhenNameIsEmptyString()
        {
            var session = CreateSession();
            session.AddMonster("Monster1", 10);
            Assert.Throws<ArgumentException>(() => session.UpdateMonster(session.AddMonster("Monster1", 10).Id, name: ""));
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
        public void DoNotChangeName_WhenUpdatesWithSameName()
        {
            var session = CreateSession();
            var monster = session.AddMonster("Monster1", 10);
            session.UpdateMonster(monster.Id, name: "Monster1", currentHp: 5);
            Assert.Equal("Monster1", monster.Name);
            Assert.Equal(5, monster.CurrentHp);
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

        [Fact]
        public void Throws_WhenNewNameAlreadyExists()
        {
            var session = CreateSession();
            var monster1 = session.AddMonster("Monster1", 10);
            var monster2 = session.AddMonster("Monster2", 10);
            Assert.Throws<ArgumentException>(() => session.UpdateMonster(monster1.Id, name: "Monster2"));
        }
    }
}