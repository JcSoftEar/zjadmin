package com.zjadmin.dto;

import lombok.Data;
import lombok.NoArgsConstructor;
import lombok.AllArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class ApiResponse<T> {

    private int code = 200;
    private String message = "成功";
    private T data;
    private Integer total;

    public static <T> ApiResponse<T> success(T data) {
        return new ApiResponse<>(200, "成功", data, null);
    }

    public static <T> ApiResponse<T> success(T data, String message) {
        return new ApiResponse<>(200, message, data, null);
    }

    public static <T> ApiResponse<T> successWithTotal(T data, int total) {
        return new ApiResponse<>(200, "成功", data, total);
    }

    public static <T> ApiResponse<T> successWithTotal(T data, int total, String message) {
        return new ApiResponse<>(200, message, data, total);
    }

    public static <T> ApiResponse<T> error(String message) {
        return new ApiResponse<>(500, message, null, null);
    }

    public static <T> ApiResponse<T> error(String message, int code) {
        return new ApiResponse<>(code, message, null, null);
    }

    public static <T> ApiResponse<T> unauthorized(String message) {
        return new ApiResponse<>(401, message, null, null);
    }

    public static <T> ApiResponse<T> forbidden(String message) {
        return new ApiResponse<>(403, message, null, null);
    }
}
