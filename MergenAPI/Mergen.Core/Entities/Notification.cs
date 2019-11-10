using System;
using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class Notification : Entity
    {
        public long AccountId { get; set; }
        public NotificationTypeIds NotificationTypeId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }


    }

    public class NotificationData : Notification
    {
    }

    public class SkyChangedNotificationData : NotificationData
    {
        public int OldSky { get; set; }
        public int NewSky { get; set; }
    }

    public class LevelChangedNotificationData : NotificationData
    {
        public int OldLevel { get; set; }
        public int NewLevel { get; set; }
    }

    public class BattleCompletedNotificationData : NotificationData
    {
        public long BattleId { get; set; }
    }

    public class GameTurnNotificationData : NotificationData
    {
        public long BattleId { get; set; }
        public long GameId { get; set; }
    }

    public class AchievementUnlockedNotificationData : NotificationData
    {
        public long AchievementTypeId { get; set; }
    }

    public enum NotificationTypeIds
    {
        General = 1,
        GameTurn = 2,
        BattleCompleted = 3,
        SkyChanged = 4,
        LevelChanged = 5,
        AchievementUnlocked = 6
    }
}