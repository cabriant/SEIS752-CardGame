//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SEIS752CardGame.Business
{
    using System;
    using System.Collections.Generic;
    
    public partial class user
    {
        public user()
        {
            this.player_game = new HashSet<player_game>();
            this.user_pwd_reset = new HashSet<user_pwd_reset>();
            this.poker_table = new HashSet<poker_table>();
        }
    
        public string user_id { get; set; }
        public int account_type { get; set; }
        public string oauth_user_id { get; set; }
        public string email { get; set; }
        public string user_pwd { get; set; }
        public string display_name { get; set; }
        public string phone_number { get; set; }
        public int user_type { get; set; }
        public string oauth_auth_token { get; set; }
        public string oauth_refresh_token { get; set; }
        public int account_value { get; set; }
    
        public virtual ICollection<player_game> player_game { get; set; }
        public virtual ICollection<user_pwd_reset> user_pwd_reset { get; set; }
        public virtual ICollection<poker_table> poker_table { get; set; }
    }
}
