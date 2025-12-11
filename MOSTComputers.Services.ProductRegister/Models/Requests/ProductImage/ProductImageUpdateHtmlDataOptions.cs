using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOSTComputers.Services.ProductRegister.Models.Requests.ProductImage;
public readonly struct UpdateHtmlDataToMatchCurrentProductData { }

public readonly struct DoNotUpdateHtmlData { }

public class UpdateToCustomHtmlData
{
    public string? HtmlData { get; init; }
}