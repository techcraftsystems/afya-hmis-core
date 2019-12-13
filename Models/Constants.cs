﻿namespace AfyaHMIS.Models
{
    public static class Constants
    {
        public const long ENCOUNTER_REGISTRATION = 1;
        public const long ENCOUNTER_VISIT = 2;

        //ROOMS
        public const long ROOM_TRIAGE = 1;

        //VISIT
        public const long VISIT_FACILITY = 1;

        //STATUS
        public const long STATUS_NEW = 0;
        public const long STATUS_ACTIVE = 1;
        public const long STATUS_ADMIT = 2;
        public const long STATUS_DISCHARGE = 3;
        public const long STATUS_DIED = 99;
        
        //CONCEPTS
        public const long MEDICO_LEGAL = 11;
        public const long REFERRAL_TYPE = 7;
    }
}
