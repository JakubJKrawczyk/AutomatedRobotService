using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutomateRobotService.FilesTemplates
{
    internal class ProgramTemplate
    {

        public Regex FileTemplate { get; set; }
        public string exapleName { get; set; } = "SKYWALK ŚWIERADÓW ZDRÓJ SP.Z O.O._16.06.2023 15-30-42";
        public ProgramTemplate()
        {
            FileTemplate = new Regex("[a-zA-z]+_[0-9.]+");
        }
        public bool Match(string fileName)
        {
            return FileTemplate.Match(fileName).Success;
        }
    }
}
