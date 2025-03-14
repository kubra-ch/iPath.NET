﻿using iPath.Data.Entities;

namespace iPath.Application.Features;

public class NodeListDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    public string CaseType { get; set; }
    public string AccessionNo { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? LastAnnotationOn { get; set; }
    public UserDTO Owner { get; set; }
}
