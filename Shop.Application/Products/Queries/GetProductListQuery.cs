using System;
using System.Collections.Generic;
using System.Text;
using Shop.Application.Common.Pagination;

namespace Shop.Application.Products.Queries;

public record GetProductListQuery(PageRequest Pagination);
