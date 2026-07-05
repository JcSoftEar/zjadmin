package com.zjadmin.mapper;

import com.baomidou.mybatisplus.core.mapper.BaseMapper;
import com.zjadmin.entity.SysConfig;
import org.apache.ibatis.annotations.Mapper;

@Mapper
public interface ConfigMapper extends BaseMapper<SysConfig> {
}
