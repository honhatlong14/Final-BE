using Common.Constants;

namespace Data.Entities;

public class Category : BaseEntity.BaseEntity
{
    public string CategoryName { set; get; }
    public IList<BookCategory>? BookCategories { set; get; }
    public StringEnum.CategoryStatus CategoryStatus { set; get; }
}