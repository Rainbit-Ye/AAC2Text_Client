namespace DataTable
{
    public class IconLabelDataTable : DataTableBase<IconLabelDataTable>
    {
        public string IconLabel { get; set; }
        public string SemanticType { get; set; }
        
        public string Category{ get; set; }
        public override string GetTableName()
        {
            return "IconLabelDataTable";
        }
    }
}