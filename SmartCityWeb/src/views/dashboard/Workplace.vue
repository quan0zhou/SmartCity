<template>
  <page-header-wrapper :breadcrumb="true" :title="false">

    <template>
      <a-row :gutter="24">
        <a-col :sm="24" :md="12" :xl="6" :style="{ marginBottom: '24px' }">
          <chart-card :loading="loading" title="今日已预订金额" :total="dashboard.bookedMoney" class="first">
            <a-tooltip title="预订金额" slot="action">
              <!-- <a-icon type="info-circle-o" /> -->
            </a-tooltip>
            <!-- <template slot="footer">测试<span>￥ 234.56</span></template> -->
          </chart-card>
        </a-col>
        <a-col :sm="24" :md="12" :xl="6" :style="{ marginBottom: '24px' }">
          <chart-card :loading="loading" title="今日已预订笔数" :total="dashboard.bookedCount" class="second">
            <a-tooltip title="预订笔数" slot="action">
              <!-- <a-icon type="info-circle-o" /> -->
            </a-tooltip>

            <!-- <template slot="footer">{{ $t('dashboard.analysis.day-visits') }}<span> {{ '1234' | NumberFormat }}</span></template> -->
          </chart-card>
        </a-col>
        <a-col :sm="24" :md="12" :xl="6" :style="{ marginBottom: '24px' }">
          <chart-card :loading="loading" title="今日已退款金额" :total="dashboard.refundedMoney" class="third">
            <a-tooltip title="退款金额" slot="action">
              <!-- <a-icon type="info-circle-o" /> -->
            </a-tooltip>
            <!-- <template slot="footer">{{ $t('dashboard.analysis.conversion-rate') }} <span>60%</span></template> -->
          </chart-card>
        </a-col>
        <a-col :sm="24" :md="12" :xl="6" :style="{ marginBottom: '24px' }">
          <chart-card :loading="loading" title="今日已退款笔数" :total="dashboard.refundedCount" class="fourth">
            <a-tooltip title="退款笔数" slot="action">
              <!-- <a-icon type="info-circle-o" /> -->
            </a-tooltip>
          </chart-card>
        </a-col>
      </a-row>
    </template></page-header-wrapper>
</template>

<script>
import { PageHeaderWrapper } from '@ant-design-vue/pro-layout'
import {
  ChartCard
} from '@/components'

export default {
  name: 'Workplace',
  components: {
    PageHeaderWrapper,
    ChartCard
  },
  data () {
    return {
      loading: true,
      dashboard: {}
    }
  },
  computed: {

  },
  created () {
    this.$http.get('order/DashBoard').then(res => {
      this.loading = false
       this.dashboard.bookedCount = res.bookedCount
       this.dashboard.bookedMoney = '￥' + res.bookedMoney
       this.dashboard.refundedCount = res.refundedCount
       this.dashboard.refundedMoney = '￥' + res.refundedMoney
    })
  },
  mounted () {

  },
  methods: {

  }
}
</script>

<style lang="less" scoped>
.ant-card.first{
  background-color: #2096fa;
  /deep/ span.chart-card-title{
     color:rgb(255, 255, 255);
  }
  /deep/ .total{
     color:rgb(255, 255, 255);
  }
   /deep/ .chart-card-footer{
    border-top: none;
  }
}
.ant-card.second{
  background-color: #4f74ecd8;
  /deep/ span.chart-card-title{
     color:rgb(255, 255, 255);
  }
  /deep/ .total{
     color:rgb(255, 255, 255);
  }
   /deep/ .chart-card-footer{
    border-top: none;
  }
}
.ant-card.third{
  background-color: #ec3b53cb;
  /deep/ span.chart-card-title{
     color:rgb(255, 255, 255);
  }
  /deep/ .total{
     color:rgb(255, 255, 255);
  }
   /deep/ .chart-card-footer{
    border-top: none;
  }
}
.ant-card.fourth{
  background-color: #c009f8c5;
  /deep/ span.chart-card-title{
     color:rgb(255, 255, 255);
  }
  /deep/ .total{
     color:rgb(255, 255, 255);
  }
  /deep/ .chart-card-footer{
    border-top: none;
  }
}
</style>
