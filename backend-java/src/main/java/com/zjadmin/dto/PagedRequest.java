package com.zjadmin.dto;

import lombok.Data;

@Data
public class PagedRequest {
    private int page = 1;
    private int pageSize = 10;
}
