export type Json =
  | string
  | number
  | boolean
  | null
  | { [key: string]: Json | undefined }
  | Json[]

export type Database = {
  public: {
    Tables: {
      customer_services: {
        Row: {
          created_at: string
          customer_id: string
          id: string
          service_id: string
        }
        Insert: {
          created_at?: string
          customer_id: string
          id?: string
          service_id: string
        }
        Update: {
          created_at?: string
          customer_id?: string
          id?: string
          service_id?: string
        }
        Relationships: [
          {
            foreignKeyName: "customer_services_customer_id_fkey"
            columns: ["customer_id"]
            isOneToOne: false
            referencedRelation: "customers"
            referencedColumns: ["id"]
          },
          {
            foreignKeyName: "customer_services_service_id_fkey"
            columns: ["service_id"]
            isOneToOne: false
            referencedRelation: "services"
            referencedColumns: ["id"]
          },
        ]
      }
      customers: {
        Row: {
          acquired_services: Json | null
          brand_name: string
          contact_email: string
          contact_name: string
          contact_phone: string | null
          created_at: string
          customer_type: string
          id: string
          updated_at: string
        }
        Insert: {
          acquired_services?: Json | null
          brand_name: string
          contact_email: string
          contact_name: string
          contact_phone?: string | null
          created_at?: string
          customer_type?: string
          id?: string
          updated_at?: string
        }
        Update: {
          acquired_services?: Json | null
          brand_name?: string
          contact_email?: string
          contact_name?: string
          contact_phone?: string | null
          created_at?: string
          customer_type?: string
          id?: string
          updated_at?: string
        }
        Relationships: []
      }
      requirements: {
        Row: {
          category: string | null
          completion_percentage: number | null
          content: string
          created_at: string
          customer_id: string | null
          customer_name: string | null
          id: string
          is_anonymous: boolean | null
          priority: string | null
          project_area: string | null
          status: string
          title: string
          updated_at: string
        }
        Insert: {
          category?: string | null
          completion_percentage?: number | null
          content: string
          created_at?: string
          customer_id?: string | null
          customer_name?: string | null
          id?: string
          is_anonymous?: boolean | null
          priority?: string | null
          project_area?: string | null
          status?: string
          title: string
          updated_at?: string
        }
        Update: {
          category?: string | null
          completion_percentage?: number | null
          content?: string
          created_at?: string
          customer_id?: string | null
          customer_name?: string | null
          id?: string
          is_anonymous?: boolean | null
          priority?: string | null
          project_area?: string | null
          status?: string
          title?: string
          updated_at?: string
        }
        Relationships: [
          {
            foreignKeyName: "requirements_customer_id_fkey"
            columns: ["customer_id"]
            isOneToOne: false
            referencedRelation: "customers"
            referencedColumns: ["id"]
          },
        ]
      }
      response_completion_rates: {
        Row: {
          average_completion_time: number | null
          created_at: string
          id: string
          survey_id: string
          total_completed: number
          total_started: number
          updated_at: string
        }
        Insert: {
          average_completion_time?: number | null
          created_at?: string
          id?: string
          survey_id: string
          total_completed?: number
          total_started?: number
          updated_at?: string
        }
        Update: {
          average_completion_time?: number | null
          created_at?: string
          id?: string
          survey_id?: string
          total_completed?: number
          total_started?: number
          updated_at?: string
        }
        Relationships: [
          {
            foreignKeyName: "response_completion_rates_survey_id_fkey"
            columns: ["survey_id"]
            isOneToOne: false
            referencedRelation: "surveys"
            referencedColumns: ["id"]
          },
        ]
      }
      services: {
        Row: {
          created_at: string
          description: string | null
          id: string
          name: string
        }
        Insert: {
          created_at?: string
          description?: string | null
          id?: string
          name: string
        }
        Update: {
          created_at?: string
          description?: string | null
          id?: string
          name?: string
        }
        Relationships: []
      }
      survey_email_logs: {
        Row: {
          created_at: string
          error_message: string | null
          id: string
          recipients: Json
          sent_at: string
          status: string
          survey_id: string
        }
        Insert: {
          created_at?: string
          error_message?: string | null
          id?: string
          recipients: Json
          sent_at?: string
          status: string
          survey_id: string
        }
        Update: {
          created_at?: string
          error_message?: string | null
          id?: string
          recipients?: Json
          sent_at?: string
          status?: string
          survey_id?: string
        }
        Relationships: [
          {
            foreignKeyName: "survey_email_logs_survey_id_fkey"
            columns: ["survey_id"]
            isOneToOne: false
            referencedRelation: "surveys"
            referencedColumns: ["id"]
          },
        ]
      }
      survey_responses: {
        Row: {
          answers: Json
          id: string
          respondent_company: string | null
          respondent_email: string | null
          respondent_name: string
          respondent_phone: string | null
          submitted_at: string
          survey_id: string
        }
        Insert: {
          answers: Json
          id?: string
          respondent_company?: string | null
          respondent_email?: string | null
          respondent_name: string
          respondent_phone?: string | null
          submitted_at?: string
          survey_id: string
        }
        Update: {
          answers?: Json
          id?: string
          respondent_company?: string | null
          respondent_email?: string | null
          respondent_name?: string
          respondent_phone?: string | null
          submitted_at?: string
          survey_id?: string
        }
        Relationships: [
          {
            foreignKeyName: "survey_responses_survey_id_fkey"
            columns: ["survey_id"]
            isOneToOne: false
            referencedRelation: "surveys"
            referencedColumns: ["id"]
          },
        ]
      }
      surveys: {
        Row: {
          created_at: string
          delivery_config: Json | null
          description: string | null
          id: string
          questions: Json
          status: string | null
          title: string
          updated_at: string
        }
        Insert: {
          created_at?: string
          delivery_config?: Json | null
          description?: string | null
          id?: string
          questions: Json
          status?: string | null
          title: string
          updated_at?: string
        }
        Update: {
          created_at?: string
          delivery_config?: Json | null
          description?: string | null
          id?: string
          questions?: Json
          status?: string | null
          title?: string
          updated_at?: string
        }
        Relationships: []
      }
    }
    Views: {
      [_ in never]: never
    }
    Functions: {
      [_ in never]: never
    }
    Enums: {
      [_ in never]: never
    }
    CompositeTypes: {
      [_ in never]: never
    }
  }
}

type PublicSchema = Database[Extract<keyof Database, "public">]

export type Tables<
  PublicTableNameOrOptions extends
    | keyof (PublicSchema["Tables"] & PublicSchema["Views"])
    | { schema: keyof Database },
  TableName extends PublicTableNameOrOptions extends { schema: keyof Database }
    ? keyof (Database[PublicTableNameOrOptions["schema"]]["Tables"] &
        Database[PublicTableNameOrOptions["schema"]]["Views"])
    : never = never,
> = PublicTableNameOrOptions extends { schema: keyof Database }
  ? (Database[PublicTableNameOrOptions["schema"]]["Tables"] &
      Database[PublicTableNameOrOptions["schema"]]["Views"])[TableName] extends {
      Row: infer R
    }
    ? R
    : never
  : PublicTableNameOrOptions extends keyof (PublicSchema["Tables"] &
        PublicSchema["Views"])
    ? (PublicSchema["Tables"] &
        PublicSchema["Views"])[PublicTableNameOrOptions] extends {
        Row: infer R
      }
      ? R
      : never
    : never

export type TablesInsert<
  PublicTableNameOrOptions extends
    | keyof PublicSchema["Tables"]
    | { schema: keyof Database },
  TableName extends PublicTableNameOrOptions extends { schema: keyof Database }
    ? keyof Database[PublicTableNameOrOptions["schema"]]["Tables"]
    : never = never,
> = PublicTableNameOrOptions extends { schema: keyof Database }
  ? Database[PublicTableNameOrOptions["schema"]]["Tables"][TableName] extends {
      Insert: infer I
    }
    ? I
    : never
  : PublicTableNameOrOptions extends keyof PublicSchema["Tables"]
    ? PublicSchema["Tables"][PublicTableNameOrOptions] extends {
        Insert: infer I
      }
      ? I
      : never
    : never

export type TablesUpdate<
  PublicTableNameOrOptions extends
    | keyof PublicSchema["Tables"]
    | { schema: keyof Database },
  TableName extends PublicTableNameOrOptions extends { schema: keyof Database }
    ? keyof Database[PublicTableNameOrOptions["schema"]]["Tables"]
    : never = never,
> = PublicTableNameOrOptions extends { schema: keyof Database }
  ? Database[PublicTableNameOrOptions["schema"]]["Tables"][TableName] extends {
      Update: infer U
    }
    ? U
    : never
  : PublicTableNameOrOptions extends keyof PublicSchema["Tables"]
    ? PublicSchema["Tables"][PublicTableNameOrOptions] extends {
        Update: infer U
      }
      ? U
      : never
    : never

export type Enums<
  PublicEnumNameOrOptions extends
    | keyof PublicSchema["Enums"]
    | { schema: keyof Database },
  EnumName extends PublicEnumNameOrOptions extends { schema: keyof Database }
    ? keyof Database[PublicEnumNameOrOptions["schema"]]["Enums"]
    : never = never,
> = PublicEnumNameOrOptions extends { schema: keyof Database }
  ? Database[PublicEnumNameOrOptions["schema"]]["Enums"][EnumName]
  : PublicEnumNameOrOptions extends keyof PublicSchema["Enums"]
    ? PublicSchema["Enums"][PublicEnumNameOrOptions]
    : never

export type CompositeTypes<
  PublicCompositeTypeNameOrOptions extends
    | keyof PublicSchema["CompositeTypes"]
    | { schema: keyof Database },
  CompositeTypeName extends PublicCompositeTypeNameOrOptions extends {
    schema: keyof Database
  }
    ? keyof Database[PublicCompositeTypeNameOrOptions["schema"]]["CompositeTypes"]
    : never = never,
> = PublicCompositeTypeNameOrOptions extends { schema: keyof Database }
  ? Database[PublicCompositeTypeNameOrOptions["schema"]]["CompositeTypes"][CompositeTypeName]
  : PublicCompositeTypeNameOrOptions extends keyof PublicSchema["CompositeTypes"]
    ? PublicSchema["CompositeTypes"][PublicCompositeTypeNameOrOptions]
    : never
