using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Models.FileManagement.Models;

public readonly struct NotSupportedFileTypeResult
{
    public required string FileExtension { get; init; }
}