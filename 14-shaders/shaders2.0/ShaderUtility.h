#include <SOIL2/SOIL2.h>
#include <GL/glew.h>
#include <gl/GL.h>
#include <gl/GLU.h>
#include <gl/freeglut.h>
#include <glm/trigonometric.hpp>
#include <glm/gtc/matrix_transform.hpp>
#include <iostream>
#include <fstream>
#include <vector>
#include <tuple>
#include <regex>

using namespace std;

class Point3D
{
public:
	float x;
	float y;
	float z;
	Point3D(float x, float y, float z)
	{
		this->x = x;
		this->y = y;
		this->z = z;
	}
	Point3D()
	{
		x = 0;
		y = 0;
		z = 0;
	}
};

class Point2D
{
public:
	float x;
	float y;
	Point2D(float x, float y)
	{
		this->x = x;
		this->y = y;
	}
	Point2D()
	{
		x = 0;
		y = 0;
	}
};

class Mesh
{
public:
	vector<Point3D> pointList;
	vector<Point3D> normalList;
	vector<Point2D> texturePoint;
	vector<vector<tuple<int, int, int>>> polygons;
	vector<int> indicesList;
	Mesh()
	{
		this->normalList = vector<Point3D>();
		this->pointList = vector<Point3D>();
		this->texturePoint = vector<Point2D>();
		this->polygons = vector<vector<tuple<int, int, int>>>();
		this->indicesList = vector<int>();
	}
	Mesh(vector<Point3D> points)
	{
		this->pointList = vector<Point3D>();

		for (int i = 0; i < points.size(); i++)
		{
			this->pointList.push_back(points[i]);
		}
	}
};

class ShaderUtils
{
private:
	GLuint vertex_shader;
	GLuint fragment_shader;

	GLuint compileSource(const char* source, GLuint shader_type)
	{
		GLuint shader = glCreateShader(shader_type);
		glShaderSource(shader, 1, &source, NULL);
		glCompileShader(shader);
		return shader;
	}

	void linkProgram()
	{
		ShaderProgram = glCreateProgram();
		glAttachShader(ShaderProgram, vertex_shader);
		glAttachShader(ShaderProgram, fragment_shader);
		glLinkProgram(ShaderProgram);
	}

public:
	GLuint ShaderProgram;

	ShaderUtils() :ShaderProgram(0) {}
	~ShaderUtils()
	{
		glUseProgram(0);
		glDeleteShader(vertex_shader);
		glDeleteShader(fragment_shader);
		glDeleteProgram(ShaderProgram);
	}

	void load(const char* vertext_src, const char* fragment_src)
	{
		vertex_shader = compileSource(vertext_src, GL_VERTEX_SHADER);
		fragment_shader = compileSource(fragment_src, GL_FRAGMENT_SHADER);
		linkProgram();
	}

	void load_vertex_shader(const char* vertext_src, const char* fragment_src)
	{
		vertex_shader = compileSource(vertext_src, GL_VERTEX_SHADER);
		fragment_shader = compileSource(fragment_src, GL_FRAGMENT_SHADER);
		linkProgram();
	}

	GLuint getIDProgram()
	{
		return ShaderProgram;
	}

	GLint getAttribLocation(const char* name)const
	{
		GLint t = glGetAttribLocation(ShaderProgram, name);
		if (t == -1)
		{
			std::cout << "could not bind attrib " << name << std::endl;
			return -1;
		}
		return t;
	}
	GLuint getUniformLocation(const char* name)const
	{
		GLint t = glGetUniformLocation(ShaderProgram, name);
		if (t == -1)
		{
			std::cout << "could not bind uniform " << name << std::endl;
			return -1;
		}
		return t;
	}
	void use() { glUseProgram(ShaderProgram); }
};

GLfloat* triangulate(vector<Point3D> verts, int size1, vector<int> inds, int size2, int& out_size)
{
	int n1 = size1;
	int n2 = size2;
	int ind = 0;
	out_size = n2 * 3;
	GLfloat* ans = new GLfloat[out_size];
	for (size_t i = 0; i < n2 / 3; i++)
	{
		for (size_t j = 0; j < 3; j++)
		{
			ans[ind] = verts[(int)inds[i * 3 + j]].x;
			ind++;
			ans[ind] = verts[(int)inds[i * 3 + j]].y;
			ind++;
			ans[ind] = verts[(int)inds[i * 3 + j]].z;
			ind++;
		}
	}
	return ans;
}

Mesh openOBJ(string filename)
{
	Point3D point(1, 2, 3);
	char buff[1000];
	ifstream fin(filename);

	regex myregex;
	int mode = 0;

	Mesh mesh;

	vector<Point2D> uv_vec;
	vector<Point3D> norm_vec;

	while (!fin.eof())
	{
		fin.getline(buff, 1000);
		string s = buff;
		if (s[0] == 'v')
		{
			if (s[1] == ' ')
			{
				myregex = regex("v (\-?\\d+,\\d+) (\-?\\d+,\\d+) (\-?\\d+,\\d+)");
				mode = 0;
			}
			else if (s[1] == 'n')
			{
				myregex = regex("vn (\-?\\d+,\\d+) (\-?\\d+,\\d+) (\-?\\d+,\\d+)");
				mode = 1;
			}
			else if (s[1] == 't')
			{
				myregex = regex("vt (\-?\\d+,\\d+) (\-?\\d+,\\d+)");
				mode = 2;
			}
		}
		else if (s[0] == 'f')
		{
			myregex = regex("f (\\d+/\\d+/\\d+) (\\d+/\\d+/\\d+) (\\d+/\\d+/\\d+)");
			mode = 3;
		}
		else
			continue;
		auto words_begin = sregex_iterator(s.begin(), s.end(), myregex);
		auto words_end = sregex_iterator();
		for (sregex_iterator i = words_begin; i != words_end; i++)
		{
			smatch match = *i;
			if (mode == 0)
			{
				Point3D p(stod(match[1]), stod(match[2]), stod(match[3]));
				mesh.pointList.push_back(p);
			}
			else if (mode == 1)
			{
				Point3D p(stod(match[1]), stod(match[2]), stod(match[3]));
				norm_vec.push_back(p);
			}
			else if (mode == 2)
			{
				Point2D p(stod(match[1]), stod(match[2]));
				uv_vec.push_back(p);
			}
			else if (mode == 3)
			{
				vector<tuple<int, int, int>> polygon = vector<tuple<int, int, int>>();
				for (int j = 1; j < match.size(); j++)
				{
					regex point = regex("(\\d+)/(\\d+)/(\\d+)");
					string s0 = match[j];
					auto matchpoint = sregex_iterator(s0.begin(), s0.end(), point);
					polygon.push_back(make_tuple(stoi((*matchpoint)[1]) - 1, stoi((*matchpoint)[2]) - 1, stoi((*matchpoint)[3]) - 1));
				}
				mesh.polygons.push_back(polygon);
			}
		}

	}
	mesh.indicesList = vector<int>(mesh.polygons.size() * 3);
	mesh.normalList = vector<Point3D>(mesh.polygons.size() * 3);
	mesh.texturePoint = vector<Point2D>(mesh.polygons.size() * 3);
	int pointer = 0;
	for (int i = 0; i < mesh.polygons.size(); i++)
	{
		for (int j = 0; j < mesh.polygons[i].size(); j++)
		{
			try
			{
				int ind1 = get<0>(mesh.polygons[i][j]);
				int ind2 = get<1>(mesh.polygons[i][j]);
				int ind3 = get<2>(mesh.polygons[i][j]);
				mesh.indicesList[pointer] = ind1;
				mesh.texturePoint[pointer] = uv_vec[ind2];
				mesh.normalList[pointer] = norm_vec[ind3];
				++pointer;
			}
			catch (exception){}
		}
	}
	fin.close();
	return mesh;
}
