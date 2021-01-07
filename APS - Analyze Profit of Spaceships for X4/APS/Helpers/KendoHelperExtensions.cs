using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using APS.Helpers.Parameters;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace APS.Helpers
{
    public static class KendoHelperExtensions
    {
        #region DefaultGridConfiguration

        public static GridBuilder<T> StandardGridConfiguration<T>(this GridBuilder<T> builder) where T : class
        {
            builder
                .HtmlAttributes(new  { @style = "height:100%;" } )
                .Selectable()
                .Navigatable(true)
                .Reorderable(r => r.Columns(true))
                .Resizable(r => r.Columns(true))
                .Scrollable(s => s.Height("100%"))
                .Sortable()
                .ColumnMenu(x=> x.Filterable(false).Sortable(false))
                .Events(events => events.Save("savePopup")
                        .DataBound("dataBound_Handler"))
                .Filterable(filtrable => filtrable.Mode(GridFilterMode.Row).Extra(false).Operators(o => o.ForString(s => s.Clear().Contains("Contient"))
                                                                                                        .ForDate(d=>d.Clear().IsGreaterThanOrEqualTo("plus grand que")
                                                                                                                             .IsLessThanOrEqualTo("plus petit que")))
                                                                                                       )
                .Pageable(pageable => pageable
                    .Refresh(true)
                    .PageSizes(new[] {10,11,12,13,14,15, 17, 30, 40, 100, 200, 500})
                    .ButtonCount(5)
                );
            return builder;
        }

        #endregion

        #region ExportationToolbar

        // Adds PDF / Excel export buttons only
        public static GridBuilder<T> ExportationPdf<T>(this GridBuilder<T> builder, string documentName) where T : class
        {
            TagBuilder wrapper = new TagBuilder("div");
            
                TagBuilder pdfLink = CreateToolbarLink(ExportFormat.Pdf, string.Empty);

                TagBuilder pdfCssLink = new TagBuilder("link");
                pdfCssLink.Attributes.AddRange(new []
                {
                    new KeyValuePair<string, string>("rel", "stylesheet"),
                    new KeyValuePair<string, string>("type", "text/css"),
                    new KeyValuePair<string, string>("href", "/css/pdf.css")
                });

                wrapper.InnerHtml.AppendHtml(pdfLink);
                wrapper.InnerHtml.AppendHtml(pdfCssLink);

            builder.Pdf(pdf => pdf
                .AllPages()
                .PaperSize("A4")
                .Scale(0.5)
                .Margin("2cm", "1cm", "1cm", "1cm")
                .Template(CreatePdfTemplate(documentName))
                .Landscape()
                .FileName(string.Format("{0}_{1}.pdf", documentName,
                    DateTime.Now.ToString("ddMMyyyy")))
            );

            string htmlOutput;
            using (StringWriter writer = new StringWriter())
            {
                wrapper.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
                htmlOutput = writer.ToString();
            }
            
            builder.ToolBar(toolbar => toolbar.ClientTemplate(htmlOutput));
            
            return builder;
        }
        
        private static TagBuilder CreateToolbarLink(ExportFormat exportFormat, string href)
        {
            TagBuilder link = new TagBuilder("a");
            link.Attributes.AddRange(new[]
            {
                new KeyValuePair<string, string>("class", "k-button k-button-icontext"),
                new KeyValuePair<string, string>("style", "float: right;"), 
                new KeyValuePair<string, string>("href", href)
            });

            TagBuilder span = new TagBuilder("span");

            switch (exportFormat)
            {
                case ExportFormat.Excel:
                    span.AddCssClass("k-icon k-i-file-excel");
                    
                    break;
                case ExportFormat.Pdf:
                    link.AddCssClass("k-grid-pdf");
                    span.AddCssClass("k-icon k-i-file-pdf");
                    break;
                default:
                    throw new Exception("Type de ExportFormat non supporté.");
            }
            
            link.InnerHtml.AppendHtml(span);

            link.InnerHtml.Append(exportFormat.ToString());

            return link;
        }

        private static string CreatePdfTemplate(string documentName)
        {
            TagBuilder wrapper = new TagBuilder("div");
            wrapper.AddCssClass("page-template");

            // Header
            TagBuilder header = new TagBuilder("div");
            header.AddCssClass("header");

            // 1er div du header (Date)
            TagBuilder headerDiv1 = new TagBuilder("div");
            headerDiv1.AddCssClass("col-md-4");

            TagBuilder logo = new TagBuilder("img");
            logo.Attributes.Add("src", "/images/logo.png");
            logo.RenderSelfClosingTag();
            
            headerDiv1.InnerHtml.AppendHtml(logo);

            // 2eme div du header (Title)
            TagBuilder headerDiv2 = new TagBuilder("div");
            headerDiv2.AddCssClass("col-md-4");

            TagBuilder headerDiv2Title = new TagBuilder("h3");
            headerDiv2Title.InnerHtml.Append(documentName);
            headerDiv2.InnerHtml.AppendHtml(headerDiv2Title);

            // 3eme div du header (Date)
            TagBuilder headerDiv3 = new TagBuilder("div");
            headerDiv3.AddCssClass("col-md-4");

            TagBuilder headerDiv3Span = new TagBuilder("span");
            headerDiv3Span.InnerHtml.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

            headerDiv3.InnerHtml.AppendHtml(headerDiv3Span);

            // Header (on inclut les divs)
            header.InnerHtml.AppendHtml(headerDiv1);
            header.InnerHtml.AppendHtml(headerDiv2);
            header.InnerHtml.AppendHtml(headerDiv3);

            // Footer
            TagBuilder footer = new TagBuilder("div");
            footer.AddCssClass("footer");
            footer.InnerHtml.Append("Page #: pageNum # of #: totalPages #"); 

            // On inclut le header + footer dans le wrapper
            wrapper.InnerHtml.AppendHtml(header);
            wrapper.InnerHtml.AppendHtml(footer);

            string htmlOutput;
            using (StringWriter writer = new StringWriter())
            {
                wrapper.WriteTo(writer, System.Text.Encodings.Web.HtmlEncoder.Default);
                htmlOutput = writer.ToString();
            }

            return htmlOutput;
        }

        #endregion
    }
}
