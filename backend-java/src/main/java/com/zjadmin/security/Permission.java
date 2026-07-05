package com.zjadmin.security;

import java.lang.annotation.*;

/**
 * 权限注解 - 标注在 Controller 方法上，指定所需的权限码
 */
@Target(ElementType.METHOD)
@Retention(RetentionPolicy.RUNTIME)
@Documented
public @interface Permission {
    String value();
}
