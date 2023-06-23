using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutomateRobotService.FilesTemplates
{
    internal class EnployeeTemplate
    {
        public Regex FileTemplate { get; set; }

        private string exapleName { get; set; } = "BJCTOOLSPOLAND_SPZOO_FK_backup_2023_06_17_024138_1657436";
        public EnployeeTemplate()
        {
            FileTemplate = new Regex("[a-zA-z]+_[0-9.]+");
        }
        public bool Match(string fileName)
        {
            return FileTemplate.Match(fileName).Success;
        }
    }
}
