package com.zjadmin;

import org.mybatis.spring.annotation.MapperScan;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

@SpringBootApplication
@MapperScan("com.zjadmin.mapper")
public class ZjAdminApplication {

    public static void main(String[] args) {
        System.out.println("""

            ╔══════════════════════════════════════════════════════════════════════════╗
            ║  Author   : James YinG                    Email : james@taogame.com     ║
            ║  GitHub   : github.com/JcSoftEar/zjadmin                                ║
            ║  业务：软件开发 | 定制 | 修复 | 部署专业接单，品质保障，欢迎合作！       ║
            ╚══════════════════════════════════════════════════════════════════════════╝

            ZJAdmin Java Backend starting...
            """);
        SpringApplication.run(ZjAdminApplication.class, args);
    }
}
